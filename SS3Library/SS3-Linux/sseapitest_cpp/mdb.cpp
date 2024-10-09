#ifdef __UNIXLINUX
#include <sys/types.h>
#include <dirent.h>
#else
#include "stdafx.h"
#include <Windows.h>
#include <tchar.h>
#endif //__UNIXLINUX
#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <iostream>
#include "mdb.h"

MDB::MDB() :
    m_pTemplate(NULL)
{
    Clear();
}

MDB::~MDB()
{
    Clear();
}

void MDB::Clear()
{
    if (m_pTemplate)
        free(m_pTemplate);
    m_pTemplate = NULL;
    m_templateAllocNum = 0;
    m_count = 0;
}

void MDB::Create(int numTemplates)
{
    Clear();

    m_templateAllocNum = numTemplates;
    m_pTemplate = (TEMPLATE*)malloc(sizeof(TEMPLATE) * numTemplates);
    std::cout << "total db size = " << numTemplates << std::endl;
}

bool MDB::Load(const char *pszPathName)
{
    int numFiles = 0;
    char szPathName[300] = {0};
    FILE *fp = NULL;
    TEMPLATE minTemplate = {0};
#ifdef __UNIXLINUX
    DIR *dp;
    struct dirent *dirp;
    if((dp  = opendir(pszPathName)) == NULL) {
        //cout << "Error(" << errno << ") opening " << dir << endl;
        printf("Error (%d) opening %s\n",errno,pszPathName);
        return errno;
    }

    while ((dirp = readdir(dp)) != NULL) {
        //files.push_back(string(dirp->d_name));
        //printf("Found %s\n",dirp->d_name);
        if (strcmp(dirp->d_name,".") && strcmp(dirp->d_name,".."))
        numFiles++;
    }
    closedir(dp);
    printf("Found %d files\n",numFiles);

    // Allocate memory
    Create(numFiles);

    // Add templates
    numFiles = 0;

    if((dp  = opendir(pszPathName)) == NULL) {
        //cout << "Error(" << errno << ") opening " << dir << endl;
        printf("Error (%d) opening %s\n",errno,pszPathName);
        return errno;
    }

    while ((dirp = readdir(dp)) != NULL) {
        //files.push_back(string(dirp->d_name));
        if (strcmp(dirp->d_name,".") && strcmp(dirp->d_name,".."))
        {
            sprintf(szPathName, "%s/%s", pszPathName, dirp->d_name);
            //printf("Processing %s\n",szPathName);
            fp = fopen(szPathName, "rb");
            if (fp) {
                printf("%d adding %s\n", numFiles + 1, dirp->d_name);
                //std::cout << numFiles + 1 << " adding " << find_data.cFileName << std::endl;
                int bytesread = fread(minTemplate.TBuffer, SG_TEMPLATE_SIZE, 1, fp);
                //printf("Requested %d bytes -- Read %d bytes\n",SG_TEMPLATE_SIZE, bytesread);
                //for (int i=0; i < SG_TEMPLATE_SIZE; ++i)
                //    printf ("%02X ", minTemplate.TBuffer[i]);
                //printf("\n");
                strcpy(minTemplate.Filename, dirp->d_name);
                minTemplate.Index = numFiles++;

                Add(&minTemplate);
                fclose(fp);
            }
        }
    }
    closedir(dp);

#else
    WIN32_FIND_DATA find_data = {0};
    char szPathName[_MAX_PATH] = {0};

    // how many .min file
    _stprintf(szPathName, "%s\\*.min", pszPathName);
    HANDLE h = FindFirstFile(szPathName, &find_data);
    while (h != INVALID_HANDLE_VALUE) {
        numFiles++;

        if (!FindNextFile(h, &find_data))
            break;
    }
    FindClose(h);

    // Allocate memory
    Create(numFiles);

    // Add templates
    numFiles = 0;
    h = FindFirstFile(szPathName, &find_data);
    while (h != INVALID_HANDLE_VALUE) {
        _stprintf(szPathName, "%s\\%s", pszPathName, find_data.cFileName);
        fp = fopen(szPathName, "rb");
        if (fp) {
            std::cout << numFiles + 1 << " adding " << find_data.cFileName << std::endl;
            fread(minTemplate.TBuffer, SG_TEMPLATE_SIZE, 1, fp);
            _tcscpy(minTemplate.Filename, find_data.cFileName);
            minTemplate.Index = numFiles++;

            Add(&minTemplate);
            fclose(fp);
        }

        if (!FindNextFile(h, &find_data))
            break;
    }
    FindClose(h);
#endif // __UNIXLINUX
    printf("Loaded %d files\n",numFiles);

    return (numFiles > 0);
}

void MDB::Add(TEMPLATE *pTemplate)
{
    memcpy(m_pTemplate[m_count].Filename, pTemplate->Filename, SIZE_FILENAME);
    m_pTemplate[m_count].Index = pTemplate->Index;
    memcpy(m_pTemplate[m_count].TBuffer, pTemplate->TBuffer, SIZE_MAX_TEMPLATE);
    m_count++;
}

TEMPLATE* MDB::operator[] (int index)
{
    if (index < m_count)
        return &m_pTemplate[index];
    else
        return NULL;
}
