// sseapitest_sample.cpp : Defines the entry point for the console application.
//
#ifdef __UNIXLINUX
#include <memory.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#else
#include "stdafx.h"
#include "loadsse.h"
#endif //__UNIXLINUX
#include <stdio.h>
#include <assert.h>
#include <vector>
#include <memory>
#include <fstream>
#include "mdb.h"
#include <SecuSearchAPI.hpp>

#define SEARCH_DB_PATH      "sample.tdb"
#define NUM_CANDIDATE       30

using namespace std;

static vector<uint8_t> readBinaryFile(const char *filename)
{
#ifdef __UNIXLINUX
   printf ("Processing file %s\n",filename);
   struct stat st;
   if (stat(filename, &st) < 0) /*failure*/
   {
      printf ("\tFILE NOT FOUND\n");
      return vector<uint8_t>();
   }
    FILE *fp = fopen(filename, "rb");
    if (fp == NULL)
    {
        printf ("\tUNABLE TO OPEN FILE\n");
        return vector<uint8_t>();

    }
    //printf ("\tFILE IS %d BYTES LONG\n", st.st_size);
    vector<uint8_t> buff;
    buff.reserve((size_t)st.st_size);
    fread(reinterpret_cast<char *>(buff.data()),st.st_size, 1, fp);
    fclose(fp);
#else
    ifstream file(filename, ios::in | ios::binary | ios::ate);
    if (!file.is_open())
        return vector<uint8_t>();

    const fpos_t size = file.tellg().seekpos();
    vector<uint8_t> buff;
    buff.reserve((size_t)size);

    file.seekg(0, ios::beg);
    file.read(reinterpret_cast<char *>(buff.data()), size);
    file.close();
#endif // __UNIXLINUX

    return std::move(buff);
}

class SSEAPITest
{
public:
    SSEAPITest() {
        initSSE();
    }

    ~SSEAPITest(){
        deinitSSE();
    }

    void initSSE() {
        m_init = false;

        // Load secusearch
        m_SecuSearch = SecuSearch_GetAPI();
        //SSELoadError result = LoadSSE(SECUSEARCH_API_DLL_NAME, &m_SecuSearch);
        //if (result != DLL_ERR_NONE)
        //{
        //    printf("%s is not loaded.\n", SECUSEARCH_API_DLL_NAME);
        //    return;
        //}

        // Set parameters for initialization of secusearch
        memset(&m_SSEParam, 0, sizeof(SSE_PARAM));

        // How many candiates as results of search
        m_SSEParam.candidateCount = NUM_CANDIDATE;

        // How many engines to be created internally.
        // If the concurrency is zero, SecuSearch automatically determines the total count of the CPU cores and use all the cores.
        m_SSEParam.concurrency = 0;

        // license file - Ask SecuGen with your volume number which bin\VolNoReader.exe returns.
        m_SSEParam.szLicenseFile = (char*) "./license.dat";

        // Whether or not to allow any amount of fingerprint rotations.
        m_SSEParam.bEnableRotation = 1;

        SSE_ERROR error = m_SecuSearch->InitializeEngine(&m_SSEParam);
        switch (error)
        {
        case FPS_ERROR_NONE:
            m_init = true;
            break;
        case FPS_ERROR_SET_LOCK_PAGE_PRIVILEGE:
            printf("Cannot enable the SE_LOCK_MEMORY privilege\n");
            break;
        case FPS_ERROR_LICENSE_LOAD:
        case FPS_ERROR_LICENSE_KEY:
        case FPS_ERROR_LICENSE_EXPIRED:
            printf("License file(%s) is missing or not valid\n", m_SSEParam.szLicenseFile);
            break;
        default:
            printf("failed to initialize SecuSearch(code = %d)\n", error);
        }
    }

    bool deinitSSE() {
        if(m_SecuSearch)
        {
            m_SecuSearch->TerminateEngine();
            //FreeSSE();
            m_SecuSearch = NULL;
        }

        m_init = false;
        return true;
    }

    // Read .min files
    void loadMDB(char* mdbPathName)
    {
        assert(m_init);

        printf("loading template files in %s\n", mdbPathName);
        bool loaded = m_mdb.Load(mdbPathName);
        assert(loaded);
        printf("mdb size = %d \n", m_mdb.Size());
        printf("loading templates done\n");
    }


    // Load internal template db into secusearch
    void loadDB()
    {
        assert(m_init);

        printf("reloading template db: %s\n", SEARCH_DB_PATH);
        SSE_ERROR error = m_SecuSearch->LoadFPDB(SEARCH_DB_PATH);
        assert(FPS_ERROR_NONE == error);

        size_t fpCount;
        m_SecuSearch->GetFPCount(&fpCount);
        printf(">>  fpcount = %d\n", (int)fpCount);
        printf("reloading templates db done\n");
    }

    // Add/Register templates to secusearch
    SSE_ERROR registerMDB()
    {
        printf("Registering templates ...>>>\n");
        int count = (int)m_mdb.Size();
        int registerCount = 0;
        SSE_ERROR error = FPS_ERROR_NONE;
        for (int i = 0; i < count; i ++) {
            SSE_TEMPLATE_ID templateID = i;
            //for (int j=0; j < SG_TEMPLATE_SIZE; ++j)
            //        printf ("%02X ", m_mdb[i]->TBuffer[j]);
            //printf("\n");
            error = m_SecuSearch->RegisterFP(m_mdb[i]->TBuffer, templateID);
            if (error != FPS_ERROR_NONE) {
                printf("Registration error %d for %d\n", error,templateID);
                //Don't exit on error
                //break;
            }
            registerCount ++;
        }

        printf("Registering templates done ");

        size_t fpCount = 0;
        error = m_SecuSearch->GetFPCount(&fpCount);
        assert(FPS_ERROR_NONE == error);
        printf(">>  fpcount = %d\n", (int)fpCount);

//getchar();
        return error;
    }

    // Add/Register templates in batches to secusearch
    // to improve the speed by using multi-cores
    SSE_ERROR registerMDBBatch()
    {
        printf("Registering templates in batches ...>>>\n");
        int count = (int)m_mdb.Size();
        int registerCount = 0;
        const int batchCount = 1000;
        std::unique_ptr<ID_TEMPLATE_PAIR[]> pairs(new ID_TEMPLATE_PAIR[batchCount]);
        SSE_ERROR error = FPS_ERROR_NONE;
        int i = 0;
        while (i < count) {
            int k;
            for (k = 0; k < batchCount && i < count; k++, i++) {
                const SSE_TEMPLATE_ID templateID = i;
                pairs[k].id = templateID;
                memcpy(pairs[k].templet, m_mdb[i]->TBuffer, SSE_TEMPLATE_SIZE);
            }
            error = m_SecuSearch->RegisterFPBatch(pairs.get(), k);
            if (error != FPS_ERROR_NONE) {
                printf("Registration error %d for %d\n", error,pairs[k].id );
                //Do not break on error
                //break;
            }
            registerCount += k;
        }

        printf("Registering templates in batches done ");

        size_t fpCount = 0;
        error = m_SecuSearch->GetFPCount(&fpCount);
        assert(FPS_ERROR_NONE == error);
        printf(">>  fpcount = %d\n", (int)fpCount);

        return error;
    }

    void Test(bool bLoadDB = false)
    {
        if (m_init == false)
            return;

        char mdbPathName[] = "../../min_data";

        SSE_ERROR error = FPS_ERROR_NONE;

        printf("API version: %s\n\n", m_SecuSearch->GetVersion());

        // Read template files
        loadMDB(mdbPathName);

        if (bLoadDB) {
            loadDB();
        } else {
            // Register
            error = registerMDB();
            assert(error == FPS_ERROR_NONE);

            // Save templates of secusearch into files
            // When shutting down secusearch, you can save templates registered
            // and then reload on start-up of secusearch by calling LoadFPDB().
            printf("\n");
            printf("write DB to %s\n", SEARCH_DB_PATH);

            error = m_SecuSearch->SaveFPDB(SEARCH_DB_PATH);
            assert(error == FPS_ERROR_NONE);
        }

        // how many templates in mdb
        int testCount = m_mdb.Size();
        printf("DB has %d templates\n", testCount);

        // how many templates in secusearch
        size_t fpCount = 0, returnedFpCount;
        error = m_SecuSearch->GetFPCount(&fpCount);
        assert(error == FPS_ERROR_NONE);
        printf("SecuSearch FP Count is %ld templates\n", fpCount);

        // List IDs of templates registered
        SSE_TEMPLATE_ID* pIDList = new SSE_TEMPLATE_ID[fpCount];
        error = m_SecuSearch->GetIDList(pIDList, fpCount, &returnedFpCount);
        assert(error == FPS_ERROR_NONE);

        for(uint32_t i = 0; i < returnedFpCount; i ++) {
            printf("id = %d\n", pIDList[i]);
        }
        delete [] pIDList;

        //
        // search test
        //
#define TEST_REMOVE 0
#if TEST_REMOVE
        // Remove templates
        for (int i = 0; i < testCount; i ++) {
            SSE_TEMPLATE_ID templateID = i;
            m_SecuSearch->RemoveFP(templateID);	// Delete one template in secusearch
        }
#else // !TEST_REMOVE
        m_SecuSearch->ClearFPDB();		// Delete all templates in secusearch
        printf("Template DB is cleared ");

        error = m_SecuSearch->GetFPCount(&fpCount);
        assert(error == FPS_ERROR_NONE);
        printf(">>  fpcount = %d\n", (int)fpCount);
#endif // !TEST_REMOVE

        // search : the candidate count must be zero because secusearch has no templates.
        for (int i =  0; i < testCount; i++) {
            SSE_TEMPLATE_ID templateID = i;
            error = m_SecuSearch->SearchFP(m_mdb[i]->TBuffer, &m_candlist);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
            }

            printf("%s : ", m_mdb[i]->Filename);
            printf("%4d : ", templateID);
            if (m_candlist.count > 0) {
                printf("%5d   ", m_candlist.candidates[0].id);
                printf("matchScore=%4d\n", m_candlist.candidates[0].matchScore);
            } else {
                printf(" no candidate\n");
            }
        }

        // add/register
        if (bLoadDB) {
            loadDB();
        }
        else {
        //    //registerMDBBatch();
            registerMDB();
        }

        //// search : scores should be high such as 9999
        for (int i =  0; i < testCount; i++) {
            SSE_TEMPLATE_ID templateID = i;
            int error = m_SecuSearch->SearchFP(m_mdb[i]->TBuffer, &m_candlist);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
            }

            printf("%s : ", m_mdb[i]->Filename);
            printf("%4d : ", templateID);
            if (m_candlist.count > 0) {
                printf("%5d   ", m_candlist.candidates[0].id);
                printf("matchScore=%4d\n", m_candlist.candidates[0].matchScore);
            } else {
                printf(" no candidate\n");
            }
        }

        // ANSI 378 and ISO 19794 standard template conversion
        printf("\nRegister and search a ANSI 378 template\n");

        const vector<uint8_t> &ansiTemplateVec = ::readBinaryFile("../../test_data/ansi378_2views.bin");
        const uint8_t *ansiTemplate = ansiTemplateVec.data();
        uint8_t sgTemplate[SSE_TEMPLATE_SIZE];
        size_t numberOfViews;

        m_SecuSearch->GetNumberOfView(ansiTemplate, SS_TEMPLATE_ANSI378, &numberOfViews);
        printf ("ANSI Template has %ld views\n",numberOfViews);

        for (uint32_t indexOfView = 0; indexOfView < numberOfViews; indexOfView++)
        {
            const SSE_TEMPLATE_ID templateId = 1234560 + indexOfView;

            error = m_SecuSearch->ExtractTemplate(ansiTemplate, SS_TEMPLATE_ANSI378, indexOfView, sgTemplate);
            if (error != FPS_ERROR_NONE) {
                printf("template extraction failed error:%d\n", error);
                continue;
            }

            error = m_SecuSearch->RegisterFP(sgTemplate, templateId);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
                continue;
            }

            printf("registered the %d-th view\n", indexOfView);

            error = m_SecuSearch->SearchFP(sgTemplate, &m_candlist);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
                continue;
            }

            printf("searched the %d-th view\n", indexOfView);
            printf("expected candate: %5d\n", templateId);

            if (m_candlist.count > 0) {
                printf("candidate: %5d, ", m_candlist.candidates[0].id);
                printf("matchScore=%4d\n", m_candlist.candidates[0].matchScore);
            } else {
                printf(" no candidate\n");
            }
        }

        printf("\nRegister and search a ISO 19794 template\n");

        const vector<uint8_t> &isoTemplateVec = ::readBinaryFile("../../test_data/iso19794_1view.bin");
        const uint8_t *isoTemplate = isoTemplateVec.data();

        m_SecuSearch->GetNumberOfView(isoTemplate, SS_TEMPLATE_ISO19794, &numberOfViews);
        printf ("ISO Template has %ld views\n",numberOfViews);

        for (uint32_t indexOfView = 0; indexOfView < numberOfViews; indexOfView++)
        {
            const SSE_TEMPLATE_ID templateId = 9876540 + indexOfView;

            error = m_SecuSearch->ExtractTemplate(isoTemplate, SS_TEMPLATE_ISO19794, indexOfView, sgTemplate);
            if (error != FPS_ERROR_NONE) {
                printf("template extraction failed error:%d\n", error);
                continue;
            }

            error = m_SecuSearch->RegisterFP(sgTemplate, templateId);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
                continue;
            }

            printf("registered the %d-th view\n", indexOfView);

            error = m_SecuSearch->SearchFP(sgTemplate, &m_candlist);
            if (error != FPS_ERROR_NONE) {
                printf("search failed error:%d\n", error);
                continue;
            }

            printf("searched the %d-th view\n", indexOfView);
            printf("expected candate: %5d\n", templateId);

            if (m_candlist.count > 0) {
                printf("candidate: %5d, ", m_candlist.candidates[0].id);
                printf("matchScore=%4d\n", m_candlist.candidates[0].matchScore);
            } else {
                printf(" no candidate\n");
            }
        }

    }

    bool initialized() { return m_init; }

protected:
    bool m_init;
    SecuSearchAPI* m_SecuSearch;
    CAND_LIST m_candlist;
    MDB m_mdb;
    SSE_PARAM m_SSEParam;
};  // SSEAPITest

///////////////////////////////////////////////
#ifdef __UNIXLINUX
int main(int argc, char* argv[])
#else
int _tmain(int argc, _TCHAR* argv[])
#endif // __UNIXLINUX
{
    bool loadDB = false;

    if (argc == 2 && !strcmp(argv[1], "--loaddb"))
        loadDB = true;
    //
    // NOTE:
    //
    printf("Note:\n");
    printf("The following must be checked before running this sample.\n");
    printf("Otherwise, it will not work properly.\n");
	printf("  -license file needed if greater than 1,000 templates; for example, license.dat\n");
    printf("  -run as administrator\n");
    printf("  -command line argument '--loaddb' to load db\n");
    printf("\n");
    printf("Press Enter key to keep running...\n");
    getchar();

    printf("+++  loadDB = [%d]\n\n",loadDB);

    SSEAPITest sse_api_test;

    if (sse_api_test.initialized())
    {
        // How to use the APIs
        sse_api_test.Test(loadDB);
    }

    printf("\nApplication is done. Press Enter key to close.\n");
    getchar();

    return 0;
}


