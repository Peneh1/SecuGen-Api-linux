#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <sys/timeb.h>
#include <sys/stat.h>
#include <dirent.h>
#include "sgfplib.h"

LPSGFPM  fplib = NULL;
#define X_SIZE 248
#define Y_SIZE 292


#define MAXPATH 255
#define PARTICIPANTID "XXXX"

int main(int argc, char* argv[])
{
  char batchtype[7], imagepath[MAXPATH], templatepath[MAXPATH], database[20], dbwidth[5], dbdepth[5];
  char commandline[MAXPATH];
  int nfingers, nsamplesperfinger;
  FILE* ou;
  FILE *fp;
  char outputfile[30];
  int i,j,k,l;
  char szInputFile1[200];
  char szInputFile2[200];
  char szInputFile3[200];
  int ret;
  struct stat filestat;
  char buffer1[400];
  char buffer2[400];
  char buffer3[400];
  DWORD score;
  long err;
  long fnmr_scores[200];
  long fmr_scores[200];

  // Load parameters
  if (argc!=4)
    { 
	  printf("\nSyntax error.\nUse: generate_fdu01 <batchtype> <templatepath> <dbdepth>\n");
	  printf("  Where batchtype = FMR    - Generate DBX_FMR.BAT\n");
	  printf("                    FNMR   - Generate DBX_FNMR.BAT\n");
	  printf("          dbdepth = number of samples per finger\n");
	  return 0;
	}

  strcpy(batchtype,argv[1]);
  strcpy(templatepath,argv[2]);
  strcpy(dbdepth,argv[3]);
  nsamplesperfinger = atoi(dbdepth); 

//  printf("Debug1\n");

  for (i=0; i<200; ++i){
    fmr_scores[i] = 0;
    fnmr_scores[i] = 0;
  }
//  printf("Debug2\n");

	err = CreateSGFPMObject(&fplib);
  err = fplib->InitEx(X_SIZE, Y_SIZE, 500);
  err = fplib->SetTemplateFormat(TEMPLATE_FORMAT_SG400);

//  printf("Debug3\n");

  //ret = stat(szInputFile1,&filestat) ;
  //if (ret != 0)
  //  fprintf(stderr, "Error getting file size for: %s\n",szInputFile1);
  //else



  if (strcmp(batchtype,"FNMR") == 0)
  {
//	  for (i=1; i<= 1000; ++i)
//  printf("Debug4\n");
	  for (i=1; i<= 40044; ++i)
	  {
	    for (j=0; j<=9; ++j)
	    {
	     sprintf(szInputFile1,"%s/%d_%d_0.min",templatepath,i,j) ;
       ret = stat(szInputFile1,&filestat) ;
       if (ret != 0)
         continue;
       else
       {
  	      sprintf(szInputFile2,"%s/%d_%d_1.min",templatepath,i,j) ;
  	      sprintf(szInputFile3,"%s/%d_%d_2.min",templatepath,i,j) ;	    
          fp=fopen(szInputFile1,"rb");	
          fread(buffer1,400,1,fp);
          fclose(fp);
          fp=fopen(szInputFile2,"rb");	
          fread(buffer2,400,1,fp);
          fclose(fp);
          fp=fopen(szInputFile3,"rb");	
          fread(buffer3,400,1,fp);
          fclose(fp);
          score = 0;
          ret = fplib->GetMatchingScore((BYTE*)buffer1, (BYTE*)buffer2, &score);	    
  			  printf("%s  %s [%d]\n",szInputFile1,szInputFile2,score);
  			  ++fnmr_scores[score];
          ret = fplib->GetMatchingScore((BYTE*)buffer1, (BYTE*)buffer3, &score);	    
  			  printf("%s  %s [%d]\n",szInputFile1,szInputFile3,score);
  			  ++fnmr_scores[score];
          ret = fplib->GetMatchingScore((BYTE*)buffer1, (BYTE*)buffer2, &score);	    
  			  printf("%s  %s [%d]\n",szInputFile2,szInputFile3,score);
  			  ++fnmr_scores[score];
  			}
      }
	  }
	  strcpy(outputfile,"FNMR_FDU01.XLS");
    ou=fopen(outputfile,"w+");
    for (i=0; i<200; ++i)
  			 fprintf(ou,"%ld\n",fnmr_scores[i]);
  	  fclose(ou);
  }

  if (strcmp(batchtype,"FMR") == 0)
  {
//  printf("Debug5\n");
//	  for (i=1; i<= 100; ++i)	  
	  for (i=1; i<= 40044; ++i)
	  {
	    for (j=0; j<= 9; ++j)
	    {
	     sprintf(szInputFile1,"%s/%d_%d_0.min",templatepath,i,j) ;
       ret = stat(szInputFile1,&filestat) ;
       if (ret != 0)
         continue;
       else
       {

//  printf("Debug6\n");
          fp=fopen(szInputFile1,"rb");	
          fread(buffer1,400,1,fp);
          fclose(fp);
//  printf("Debug7\n");

//    		  for (k=i+1; k<= 100; ++k)
    		  for (k=i+1; k<= 40044; ++k)
    		  {
      	    for (l=0; l<= 9; ++l)
      	    {
      	     sprintf(szInputFile2,"%s/%d_%d_0.min",templatepath,k,l) ;
             ret = stat(szInputFile2,&filestat) ;
             if (ret != 0)
               continue;
             else
             {
//  printf("Debug8\n");
                fp=fopen(szInputFile2,"rb");	
                fread(buffer2,400,1,fp);
                fclose(fp);
                ret = fplib->GetMatchingScore((BYTE*)buffer1, (BYTE*)buffer2, &score);	    
//  printf("Debug9\n");
        			  printf("%s  %s [%d]\n",szInputFile1,szInputFile2,score);
  			        ++fmr_scores[score];
             }    
    		    }
  		    }
		    }
	    }
    }
	  strcpy(outputfile,"FMR_FDU01.XLS");
    ou=fopen(outputfile,"w+");
    for (i=0; i<200; ++i)
  			 fprintf(ou,"%ld\n",fmr_scores[i]);
  	fclose(ou);
  }
//  printf("Debug10\n");
  if (strcmp(batchtype,"FMR2") == 0)
  {
  printf("FMR2 Loading templates\n");
    //char templateBuffer[1000000][400];
    char templateBuffer[20000][400];
    int templateCount = 0;
    for (i=1; i<= 40044; ++i)
	  {
	    for (j=0; j<= 9; ++j)
	    {
//  printf("Debug12\n");
	     sprintf(szInputFile1,"%s/%d_%d_0.min",templatepath,i,j) ;
       ret = stat(szInputFile1,&filestat) ;
       if (ret != 0)
         continue;
       else
       {
          fp=fopen(szInputFile1,"rb");	
          fread(templateBuffer[templateCount],400,1,fp);
          fclose(fp);
          ++templateCount;
		    }
	    }
    }
    for (i=0; i<templateCount-1; ++i)
    {
       for (j=i+1; j<templateCount; ++j)
       {
            ret = fplib->GetMatchingScore((BYTE*)templateBuffer[i], (BYTE*)templateBuffer[j], &score);	    
    			  printf("%ld  %ld [%d]\n",i,j,score);
		        ++fmr_scores[score];
       }
    }
	  strcpy(outputfile,"FMR2_FDU01.XLS");
    ou=fopen(outputfile,"w+");
    for (i=0; i<200; ++i)
  			 fprintf(ou,"%ld\n",fmr_scores[i]);
  	fclose(ou);
  }

  return 0;
}
