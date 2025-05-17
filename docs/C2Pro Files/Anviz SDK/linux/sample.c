#include "crosschex.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <time.h>

#include "arpa/inet.h"
#include <sys/time.h>
#include "unistd.h"
#include <pthread.h>

#include <iconv.h>
#include <sys/stat.h>



#define DBG_INFO(fmt,...) {char tmp[4096];snprintf(tmp,4096,"[%ld DBG_INFO %s] ------ " fmt "\r\n",time(NULL),__FUNCTION__,##__VA_ARGS__); printf("%s\n",tmp);};
#define DBG_ERR(fmt,...) {char tmp[4096];snprintf(tmp,4096,"[%ld DBG_ERROR %s] ------ " fmt "\r\n",time(NULL),__FUNCTION__,##__VA_ARGS__); printf("%s\n",tmp);};

#define DISPOSE_THREAD

// #include <string.h>

typedef struct 
{
    void *Handle;
    char *RecBuff;
    int  BuffLen;
    int ThreadFlag;
    CCHEX_RET_DEV_LOGIN_STRU *DevInfo[2048];
}TEST_INFO_STRU; 


typedef void* ( *CBFuncThreadBody)(void *);

#define STDIN 0
//卡号转数组
void cchex_int_to_array(unsigned int in_data, unsigned char *out_data, int len)
{
    int i;
    for (i = 0; i < len; i++)
    {
        out_data[i] = (in_data & (0xFF << (8 * (len - i - 1)))) >> (8 * (len - i - 1));
    }
}

int date_array_to_utc(unsigned char *Date)
{
    int time = 946742400;//2000.01.02
    time+= Date[0] << 24;
    time+= Date[1] << 16;
    time+= Date[2] << 8;
    time+= Date[3] << 0;
    return time;
}

//密码转数组
void cchex_pwd_int_to_array(unsigned int in_pwd, unsigned char *out_pwd)
{
    out_pwd[0] += (in_pwd & 0x0F0000) >> 16;
    out_pwd[1] = (in_pwd & 0xFF00) >> 8;
    out_pwd[2] = in_pwd & 0xFF;
}
//员工号数组转数字
long long EmployeeID2Longlong(char *EmployeeId)
{
    long long ret = 0;
    int i;
    char *temp = (char*)&ret;
    for (i = 0; i < 5; i++)
    {
        temp[8 - 4 - i] = EmployeeId[i];
    }
    return ret;
}
//员工号数字转数组
void Longlong2EmployeeID(char *EmployeeId ,long long EmployeeNum)
{
    int i;
    for (i = 0; i < 5; i++)
    {
        EmployeeId[i] = (EmployeeNum >> (8 * (5 - i - 1))) & 0xFF;
    }
}
//Unicode编码字节序互换
void UnicodeNameChangeByte(char *NameStr,int Len)
{
    char temp = 0; 
    int i;
    for (i = 0; i+1 < Len; i+=2)
    {
        temp = NameStr[i];
        NameStr[i] = NameStr[i + 1];
        NameStr[i + 1] = temp;
    }
}


//Temperatureto float
float TemperatureArrayToFloat(char* Temp)
{
    int temp = (Temp[0]<< 8)+ (Temp[1]<< 0);
    float tt = ((float)temp)/10;
    return tt;
}


void utf8_2_unicode(char* pInput,char *Outbuf,int Outlen)
{
    char *encTo = "UNICODE//IGNORE";
  /* 源编码 */
  char *encFrom = "UTF-8";
    size_t outlen = Outlen;
  /* 获得转换句柄
   *@param encTo 目标编码方式
   *@param encFrom 源编码方式
   *
   * */
  iconv_t cd = iconv_open (encTo, encFrom);
  if (cd == (iconv_t)-1)
  {
      perror ("iconv_open");
  }

  /* 需要转换的字符串 */
//   char inbuf[1024] = "abcdef哈哈哈哈行"; 
  size_t srclen = strlen (pInput);
  /* 打印需要转换的字符串的长度 */
  printf("srclen=%d\n", (int)srclen);

  /* 存放转换后的字符串 */
  memset (Outbuf, 0, Outlen);

  /* 由于iconv()函数会修改指针，所以要保存源指针 */
  char *srcstart = pInput;
  char *tempoutbuf = (char*)Outbuf;

  /* 进行转换
   *@param cd iconv_open()产生的句柄
   *@param srcstart 需要转换的字符串
   *@param srclen 存放还有多少字符没有转换
   *@param tempoutbuf 存放转换后的字符串
   *@param outlen 存放转换后,tempoutbuf剩余的空间
   *
   * */
  int ret = iconv (cd, &srcstart, &srclen, &tempoutbuf, &outlen);
  if (ret == -1)
  {
      perror ("iconv");
  }
  printf ("inbuf=%s, srclen=%d, outbuf=%s, outlen=%d\n", pInput, (int)srclen, Outbuf, (int)outlen);
//   int i = 0;
//   for (i=0; i<20; i++)
//   {
//       printf("%x\n", Outbuf[i]);
//   }
/* 关闭句柄 */
  iconv_close (cd);
}

//读文件
char *read_cfg_file(const char *pPath ,int *Len)
{
    FILE *fp = NULL;
    long len = 0;
    char *p_buff = NULL;
    int file_flag = -1;
    file_flag = access(pPath, F_OK);
    if (file_flag < 0)
    {
        DBG_ERR("file: %s no exist.\n", pPath);
        return NULL;
    }

    fp = fopen(pPath, "rb"); //这里应该只读权限的
    if (fp == NULL)
    {
        DBG_ERR("file: %s open failed.\n", pPath);
        return NULL;
    }

    fseek(fp, 0, SEEK_END);
    len = ftell(fp);
    if (len == 0)
    {
        DBG_ERR("file: %s empty.\n", pPath);
        fclose(fp);
        return NULL;
    }

    fseek(fp, 0, SEEK_SET);
    p_buff = (char *)malloc(len + 1);
    if (p_buff == NULL)
    {
        DBG_ERR("p_buff malloc error.");
        fclose(fp);
        return NULL;
    }

    fread(p_buff, len, 1, fp);
    p_buff[len] = '\0';
    fclose(fp);
    *Len = len;
    return p_buff;
}

/*****************************************************************************
 * 将一个字符的UTF8编码转换成Unicode(UCS-2和UCS-4)编码.
 *
 * 参数:
 *    pInput      指向输入缓冲区, 以UTF-8编码
 *    Unic        指向输出缓冲区, 其保存的数据即是Unicode编码值,
 *                类型为unsigned long .
 *
 * 返回值:
 *    成功则返回该字符的UTF8编码所占用的字节数; 失败则返回0.
 *
 * 注意:
 *     1. UTF8没有字节序问题, 但是Unicode有字节序要求;
 *        字节序分为大端(Big Endian)和小端(Little Endian)两种;
 *        在Intel处理器中采用小端法表示, 在此采用小端法表示. (低地址存低位)
 ****************************************************************************/
int enc_utf8_to_unicode_one(const unsigned char* pInput, unsigned long *Unic,int len)
{
    if(pInput == NULL || Unic == NULL)
    {
        return -1;
    }

    // b1 表示UTF-8编码的pInput中的高字节, b2 表示次高字节, ...
    char b1, b2, b3, b4, b5, b6;

    *Unic = 0x0; // 把 *Unic 初始化为全零
    // int utfbytes = enc_get_utf8_size(*pInput);
    int utfbytes = len;
    unsigned char *pOutput = (unsigned char *) Unic;

    switch ( utfbytes )
    {
        case 0:
            *pOutput     = *pInput;
            utfbytes    += 1;
            break;
        case 2:
            b1 = *pInput;
            b2 = *(pInput + 1);
            if ( (b2 & 0xE0) != 0x80 )
                return 0;
            *pOutput     = (b1 << 6) + (b2 & 0x3F);
            *(pOutput+1) = (b1 >> 2) & 0x07;
            break;
        case 3:
            b1 = *pInput;
            b2 = *(pInput + 1);
            b3 = *(pInput + 2);
            if ( ((b2 & 0xC0) != 0x80) || ((b3 & 0xC0) != 0x80) )
                return 0;
            *pOutput     = (b2 << 6) + (b3 & 0x3F);
            *(pOutput+1) = (b1 << 4) + ((b2 >> 2) & 0x0F);
            break;
        case 4:
            b1 = *pInput;
            b2 = *(pInput + 1);
            b3 = *(pInput + 2);
            b4 = *(pInput + 3);
            if ( ((b2 & 0xC0) != 0x80) || ((b3 & 0xC0) != 0x80)
                    || ((b4 & 0xC0) != 0x80) )
                return 0;
            *pOutput     = (b3 << 6) + (b4 & 0x3F);
            *(pOutput+1) = (b2 << 4) + ((b3 >> 2) & 0x0F);
            *(pOutput+2) = ((b1 << 2) & 0x1C)  + ((b2 >> 4) & 0x03);
            break;
        case 5:
            b1 = *pInput;
            b2 = *(pInput + 1);
            b3 = *(pInput + 2);
            b4 = *(pInput + 3);
            b5 = *(pInput + 4);
            if ( ((b2 & 0xC0) != 0x80) || ((b3 & 0xC0) != 0x80)
                    || ((b4 & 0xC0) != 0x80) || ((b5 & 0xC0) != 0x80) )
                return 0;
            *pOutput     = (b4 << 6) + (b5 & 0x3F);
            *(pOutput+1) = (b3 << 4) + ((b4 >> 2) & 0x0F);
            *(pOutput+2) = (b2 << 2) + ((b3 >> 4) & 0x03);
            *(pOutput+3) = (b1 << 6);
            break;
        case 6:
            b1 = *pInput;
            b2 = *(pInput + 1);
            b3 = *(pInput + 2);
            b4 = *(pInput + 3);
            b5 = *(pInput + 4);
            b6 = *(pInput + 5);
            if ( ((b2 & 0xC0) != 0x80) || ((b3 & 0xC0) != 0x80)
                    || ((b4 & 0xC0) != 0x80) || ((b5 & 0xC0) != 0x80)
                    || ((b6 & 0xC0) != 0x80) )
                return 0;
            *pOutput     = (b5 << 6) + (b6 & 0x3F);
            *(pOutput+1) = (b5 << 4) + ((b6 >> 2) & 0x0F);
            *(pOutput+2) = (b3 << 2) + ((b4 >> 4) & 0x03);
            *(pOutput+3) = ((b1 << 6) & 0x40) + (b2 & 0x3F);
            break;
        default:
            return 0;
            break;
    }

    return utfbytes;
}


void tcb_dispose(void *ThreadHandle)       //struct Handle
{
    TEST_INFO_STRU *thread_handle = (TEST_INFO_STRU*) ThreadHandle;
    int len = thread_handle->BuffLen;
    char *buff = thread_handle->RecBuff;           // 
    int type;
    int devidx;
    int ret = 0;
    int change_flag = 0;
    void *handle = thread_handle->Handle;
    char tmp_prt[2048];
    while (1)
    {
        ret = CChex_Update(handle, &devidx, &type, buff, len);
        
        if (ret > 0)    //dispose all date
        {
            printf("~~~~~~~~~~~~~~~~ret = %d   type = %d\n",ret,type);
            switch (type)
            {
            case CCHEX_RET_RECORD_INFO_TYPE:
            case CCHEX_RET_GET_NEW_RECORD_INFO_TYPE:
            {
                CCHEX_RET_RECORD_INFO_STRU *info = (CCHEX_RET_RECORD_INFO_STRU *)buff;

                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%lld, DateUtc:%d, BackId:%d, RecType:%d, WorkType:%d, NewRecord:%d\n", 
                info->MachineId, EmployeeID2Longlong(info->EmployeeId),date_array_to_utc(info->Date), info->BackId, info->RecordType, 
                info->WorkType[2] * 0x10000 + info->WorkType[1] * 0x100 + info->WorkType[0], info->NewRecordFlag);
                DBG_INFO("%s", tmp_prt);
            }
            break;

            case CCHEX_RET_TM_ALL_RECORD_INFO_TYPE: //下载口罩测温考勤全部记录  测温或口罩记录 (FACE7EI FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3T, FDEEP3M, FDEEP3TM,FD52M,FD52TM)
            case CCHEX_RET_TM_NEW_RECORD_INFO_TYPE:
            {
                CCHEX_RET_TM_RECORD_INFO_STRU *info = (CCHEX_RET_TM_RECORD_INFO_STRU*)buff;

                printf("~~~~~temp %d %d",info->Temperature[0],info->Temperature[1]);
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%lld, DateUtc:%d, BackId:%d, RecType:%d, WorkType:%d, NewRecord:%d  Temperature:%f IsMask:%d OpenType :%d \n",
                 info->MachineId, EmployeeID2Longlong(info->EmployeeId),date_array_to_utc(info->Date), info->BackId, info->RecordType, 
                 info->WorkType[2] * 0x10000 + info->WorkType[1] * 0x100 + info->WorkType[0], info->NewRecordFlag,TemperatureArrayToFloat(info->Temperature)
                 ,info->IsMask,info->OpenType);
                DBG_INFO("%s", tmp_prt);
            }
            break;

            case CCHEX_RET_DEV_LOGIN_TYPE:
            {
                if(thread_handle->DevInfo[devidx] == NULL)
                {
                    thread_handle->DevInfo[devidx] = (CCHEX_RET_DEV_LOGIN_STRU*)calloc(1,sizeof(CCHEX_RET_DEV_LOGIN_STRU));
                }
                memcpy(thread_handle->DevInfo[devidx],buff,sizeof(CCHEX_RET_DEV_LOGIN_STRU));
                CCHEX_RET_DEV_LOGIN_STRU *info = (CCHEX_RET_DEV_LOGIN_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%d, Version:%8s, DevType:%8s, Addr:%s Login DevIdx :%d\n", info->MachineId, info->Version, info->DevType, info->Addr,devidx);
                DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);

                CChex_GetNetConfig(handle, devidx);
                //CChex_DownloadAllRecords(handle,devidx);
            }
            break;
            case CCHEX_RET_DEV_LOGOUT_TYPE:
            {
                CCHEX_RET_DEV_LOGOUT_STRU *info = (CCHEX_RET_DEV_LOGOUT_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, Version:%s, DevType:%s, Live:%d, Addr:%s Logout\n", info->MachineId, info->Version, info->DevType, info->Live, info->Addr);
                if(thread_handle->DevInfo[devidx])
                {
                    free(thread_handle->DevInfo[devidx]);
                    thread_handle->DevInfo[devidx] = NULL;
                }
                DBG_INFO("%s", tmp_prt);
            }
            break;
            case CCHEX_RET_CLINECT_CONNECT_FAIL_TYPE:
            {
                CCHEX_RET_DEV_CONNECT_STRU *info = (CCHEX_RET_DEV_CONNECT_STRU*)buff;
                printf("Connect Fail IP = %s\n",info->Addr);
            }
            break;

            case CCHEX_RET_UPLOAD_FACE_PICTURE_MODULE_TYPE:
            {
                CCHEX_UPLOAD_FACE_PICTURE_MODULE *result = (CCHEX_UPLOAD_FACE_PICTURE_MODULE*)buff;
                printf("result = %d    eid == %lld \n",result->Result ,EmployeeID2Longlong(result->EmployeeId));
            }
            break;
            case CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE:
            {
                CCHEX_RET_DEL_RECORD_OR_NEW_FLAG_STRU *result = (CCHEX_RET_DEL_RECORD_OR_NEW_FLAG_STRU*)buff;
                printf(" CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE result = %d  \n",result->Result);
            }
            break;
            case CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_TYPE:
            {
                CCHEX_RET_DEL_EMPLOYEE_INFO_STRU *info = (CCHEX_RET_DEL_EMPLOYEE_INFO_STRU *)buff;
                DBG_INFO("employee: %lld     result: %d \n", EmployeeID2Longlong(info->EmployeeId),info->Result);
            }
            break;






            case CCHEX_RET_DEV_STATUS_TYPE:
            {
                CCHEX_RET_DEV_STATUS_STRU *status = (CCHEX_RET_DEV_STATUS_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "DevStatus  Mid:%08x, EmployeeNum:%u, FingerPrtNum:%u, PasswdNum:%u, CardNum:%u, TotalRecordNum:%u\n", status->MachineId, status->EmployeeNum, status->FingerPrtNum, status->PasswdNum, status->CardNum, status->TotalRecNum);
                DBG_INFO("%s", tmp_prt);
            }
            break;


            default:
                //DBG_INFO("RET:%d, type:%d", ret, type);
                break;
            }
            // print ???
        }
        else if (0 == ret)   //without date
        {
            break;
        }
        else     //<0  buff is small ,re malloc buff
        {
            buff = malloc(len - ret );
            len = len -ret;
            change_flag = 1;
        }
    }
    if(change_flag == 1)
    {
        free(buff);
    }
}

void *tcb_workthread(void *ThreadHandle)
{
    TEST_INFO_STRU *thread_handle = (TEST_INFO_STRU*) ThreadHandle;
    while(thread_handle->ThreadFlag)
    {
        tcb_dispose(thread_handle);
        usleep(20000);
    }
    free(thread_handle->RecBuff);
    free(thread_handle);
}


void cmd_connect(void *Handle, char *Buff)
{
    int ret;
    char cmd[16];
    char ip[32];
    int port;
    //connect xxxxxx xxx xxx    //uid usr pwd
    ret = sscanf(Buff,"%s %s %d",cmd, ip, &port);
    
    if(3==ret && port >= 0 && port <= 65535)
    {
        ret = CCHex_ClientConnect(Handle,ip,port);
        printf("~~~~~~~~~~~~~~~~while\n");
        DBG_INFO("%s,%s,%d,ret:%d",cmd,ip,port,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}
void cmd_disconn(void *Handle, char *Buff)
{
    int ret;
    int dev_idx;
    char cmd[16];
    //disconn xxxx     //dev_idx
    ret = sscanf(Buff,"%s %d", cmd,&dev_idx);
    
    if(2==ret && dev_idx >= 0 && dev_idx < 2048)
    {
        ret = CCHex_ClientDisconnect(Handle,dev_idx);
        printf("~~~~~~~~~~~~~~~~while\n");
        DBG_INFO(" dev_idx %d,ret:%d",dev_idx,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}
void cmd_upload_employee(void *Handle,char * Buff) //Modify  Or Add        Password_Max len : 6   
{
    int ret;
    int dev_idx, password;
    unsigned int card_idx;
    long long employee_idx;
    char name[20],password_str[20];
    int password_len;
    char temp_u[40] = {0};
    char cmd[16];
    
    //up xxxx     //dev_idx
    printf("%s\n",Buff);
    ret = sscanf(Buff,"%s %d %lld %s %s %u",cmd, &dev_idx,&employee_idx,name,password_str,&card_idx);
    
    if(6==ret && dev_idx >= 0 && dev_idx < 2048)
    {
        password_len = strlen(password_str);
        password = atoi(password_str);

        CCHEX_EMPLOYEE2UNICODE_INFO_STRU EmployeeList;
        memset(&EmployeeList,0,sizeof(EmployeeList));
        Longlong2EmployeeID(EmployeeList.EmployeeId ,employee_idx);
        EmployeeList.Passwd[0] = password_len << 4;
        cchex_pwd_int_to_array(password,EmployeeList.Passwd);
        cchex_int_to_array(card_idx,EmployeeList.CardId, MAC_CARD_ID_LEN);
        
        utf8_2_unicode(name,EmployeeList.EmployeeName,20);
        UnicodeNameChangeByte(EmployeeList.EmployeeName,20);

        // EmployeeList.FpStatus = 0;
        EmployeeList.GroupId = 2;
        EmployeeList.Mode = 6;
        EmployeeList.DepartmentId = 0;
        EmployeeList.Special = 64;


        ret = CChex_UploadEmployee2UnicodeInfo(Handle,dev_idx,&EmployeeList, 1);
        printf("~~~~~~~~~~~~~~~~while\n");
        DBG_INFO(" dev_idx %d,ret:%d",dev_idx,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}
void cmd_upload_picture(void *Handle,char * Buff) //The image format must be JPG
{
    int ret;
    int dev_idx;
    long long employee_idx;
    char cmd[16];
    char picture_path[200] = {0};

    //up xxxx     //dev_idx
    printf("%s\n",Buff);
    ret = sscanf(Buff,"%s %d %lld %s",cmd, &dev_idx,&employee_idx,picture_path);
    
    if(4==ret && dev_idx >= 0 && dev_idx < 2048)
    {
        if(access(picture_path, F_OK) == 0)
        {
            int BuffLen = 0;
            char *PictureBuff = read_cfg_file(picture_path,&BuffLen);
            printf(" ~~~~~~%d ~~~~~~~~~\n",BuffLen);
            if(PictureBuff && BuffLen >0)
            {
                CCHEX_FACE_PICTURE_MODULE_STRU Param;
                Longlong2EmployeeID(Param.EmployeeId ,employee_idx);
                CChex_UploadFacePictureModule(Handle, dev_idx,&Param,PictureBuff,BuffLen);
                free(PictureBuff);
            }
            else
            {
                printf("path is NULL  ERROR~~~~~~\n");
            }
        }
        else
        {
            printf(" path NULL\n");
        }
        
    }
    else
    {
        DBG_INFO(" cmd_upload_picture ret:%d err",ret);
    }
}

//
void cmd_download_new_record(void *Handle, char *Buff)
{
    int ret;
    int dev_idx;
    char cmd[16];
    //disconn xxxx     //dev_idx
    ret = sscanf(Buff,"%s %d", cmd,&dev_idx);
    
    if(2==ret && dev_idx >= 0 && dev_idx < 2048)
    {
        ret = CChex_TM_DownloadAllNewRecords(Handle,dev_idx);
        DBG_INFO(" dev_idx %d,ret:%d",dev_idx,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}

//
void cmd_download_all_record(void *Handle, char *Buff)
{
    int ret;
    int dev_idx;
    char cmd[16];
    //disconn xxxx     //dev_idx
    ret = sscanf(Buff,"%s %d", cmd,&dev_idx);
    
    if(2==ret && dev_idx >= 0 && dev_idx < 2048)
    {
        ret = CChex_TM_DownloadAllRecords(Handle,dev_idx);
        DBG_INFO(" dev_idx %d,ret:%d",dev_idx,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}

//
void cmd_delete_record(void *Handle, char *Buff)
{
    int ret;
    int dev_idx;
    char cmd[16];
    //disconn xxxx     //dev_idx
    ret = sscanf(Buff,"%s %d", cmd,&dev_idx);
    
    if(2==ret && dev_idx >= 0 && dev_idx < 2048)
    {
/****************************************************************
 * 功能:删除打卡记录
 * 参数:
    CchexHandle  句柄
    DevIdx       设备当前通信号
    record.del_type  删除类型为 0:清空全部记录 1:全部新记录标记为旧记录 2:标记指定数量新记录标志为旧记录
    record.del_count  当record.del_type==2 时生效,指定标记的数量
 * 返回值: -1 :加入执行链表失败  1 :加入执行链表成功
 * 执行结果 :CChex_Update()函数类型 CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE
    若删除类型为0，返回删除全部记录条数；
    若删除类型为1，返回标记全部新记录条数；
    若删除类型为2，返回标记新记录条数。
 * 信息返回类型:CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE
 * 信息解析结构:CCHEX_RET_DEL_RECORD_OR_NEW_FLAG_STRU
****************************************************************/
        CCHEX_DEL_RECORD_OR_NEW_FLAG_INFO_STRU_EXT_INF record;
        record.del_type = 1;
        record.del_count = 0;

        ret = CChex_DeleteRecordInfo(Handle,dev_idx,&record);
        DBG_INFO(" dev_idx %d,ret:%d",dev_idx,ret);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}

//
void cmd_set_record_flag(void *Handle, char *Buff)
{
/***************************************************************************
 * after   CChex_Start();
 * para :
 *              SetRecordflag = 1  set  recordflag after download  new record;else = 0
 *              SetLogFile = 1     set some info  log to file for find  problem ;else = 0
 * if do not set "CChex_SetSdkConfig(void *CchexHandle,int SetRecordflag,int SetLogFile)", config is default   
 *                 ANVIZ_DEFAULT:
 *                                  SEATS   :SetAutodownload = 1, SetRecordflag = 1,SetLogFile = 0;
 *                                  DR      :SetAutodownload = 1, SetRecordflag = 0,SetLogFile = 0;
 *                                  COMMON  :SetAutodownload = 0, SetRecordflag = 0,SetLogFile = 0;
 *                                  BolidW2 :SetAutodownload = 1, SetRecordflag = 1,SetLogFile = 0;
*****************************************************************************/
  //~~~~~~~~~~~ If there is a record, it is automatically downloaded and marked as an old record
    int ret;
    int SetAutodownload,SetRecordflag,SetLogFile;
    char cmd[16];

    ret = sscanf(Buff,"%s %d %d %d", cmd,&SetAutodownload,&SetRecordflag,&SetLogFile);
    if(4==ret )
    {
        
        CChex_SetSdkConfig(Handle,SetAutodownload,SetRecordflag,0);
    }
    else
    {
        DBG_INFO("ret:%d err",ret);
    }
}
void cmd_help()
{
    printf("/********input \"help\"will printf all cmd ;\"exit\" to exit *************/\n");
    printf("/***************cmd example**************/\n");
    printf("help\n"); //show cmd
    printf("connect 192.168.xxx.xx 5010\n"); //cmd  ip  port
    printf("disconn 1\n");//cmd devidx
    printf("upload_emp 1 123 name 11 11\n");//cmd devidx employee_idx name password  card_idx
    printf("picture 1 123 path\n");//cmd devidx employee_idx path   (picture must is .JPG)
    printf("d_new_record 1\n");//cmd devidx 
    printf("d_all_record 1\n");//cmd devidx 
    printf("del_record 1\n");//cmd devidx 
    printf("set_record_flag 1 1 0\n");//cmd SetAutoDownload SetRecordflag SetLogFile
    printf("/***************cmd example**************/\n");
}


void cmd_analysis(void *Handle, char *Buff)
{
    int cmd_len = strlen(Buff);
    if(cmd_len>1)
    {
        if(Buff == strstr(Buff, "help"))
        {
            cmd_help();
        }
        else if(Buff == strstr(Buff, "connect"))
        {
            cmd_connect(Handle,Buff);
        }
        else if(Buff == strstr(Buff, "disconn"))
        {
            cmd_disconn(Handle,Buff);
        }
        else if(Buff == strstr(Buff, "upload_emp"))
        {
            cmd_upload_employee(Handle,Buff);
        }
        else if(Buff == strstr(Buff, "picture")) //The image format must be JPG
        {
            cmd_upload_picture(Handle,Buff);
        }
        else if(Buff == strstr(Buff, "d_new_record"))
        {
            cmd_download_new_record(Handle,Buff);
        }
        else if(Buff == strstr(Buff, "d_all_record"))
        {
            cmd_download_all_record(Handle, Buff);
        }
        else if(Buff == strstr(Buff, "del_record"))
        {
            cmd_delete_record(Handle, Buff);
        }
        else if(Buff == strstr(Buff, "set_record_flag"))
        {
            cmd_set_record_flag(Handle, Buff);
        }
        
    }
}


void my_thread_start(CBFuncThreadBody CbfThreadBody, void *Arg)
{
	pthread_t thread_id;

	if ( pthread_create( &thread_id, NULL, CbfThreadBody, (void *)Arg) )
    {
    }
    else
    {
        DBG_INFO("%s", "create thread work ok");
	}
}

int main(int argc, const char *argv[])
{
    int devidx;
    int type;
    int ret;
    int len;
    int p_port = 5010;

    CChex_Init();

    void *handle = CChex_Start();
    char *in_buf = (char *)malloc(10240);


/***************************************************************************
 * after   CChex_Start();
 * para :
 *              SetRecordflag = 1  set  recordflag after download  new record;else = 0
 *              SetLogFile = 1     set some info  log to file for find  problem ;else = 0
 * if do not set "CChex_SetSdkConfig(void *CchexHandle,int SetRecordflag,int SetLogFile)", config is default   
 *                 ANVIZ_DEFAULT:
 *                                  SEATS   :SetAutodownload = 1, SetRecordflag = 1,SetLogFile = 0;
 *                                  DR      :SetAutodownload = 1, SetRecordflag = 0,SetLogFile = 0;
 *                                  COMMON  :SetAutodownload = 0, SetRecordflag = 0,SetLogFile = 0;
 *                                  BolidW2 :SetAutodownload = 1, SetRecordflag = 1,SetLogFile = 0;
*****************************************************************************/
    if(0)   //~~~~~~~~~~~ If there is a record, it is automatically downloaded and marked as an old record
    {
        CChex_SetSdkConfig(handle,1,1,0);
    }

    printf("Ver: %d", CChex_Version());
    

    
    
    struct timeval tm;
    TEST_INFO_STRU *thread_handle = (TEST_INFO_STRU*)malloc(sizeof(TEST_INFO_STRU));
    memset(thread_handle,0,sizeof(thread_handle));
    thread_handle->Handle = handle;
    thread_handle->BuffLen = 102400;
    
    thread_handle->RecBuff = (char*)malloc(thread_handle->BuffLen);
#ifdef   DISPOSE_THREAD
    thread_handle->ThreadFlag = 1;
    my_thread_start(tcb_workthread, (void *)thread_handle); //dispose thread
#endif
    cmd_help();
    while (1)
    {
#ifndef  DISPOSE_THREAD   
        tcb_dispose(thread_handle);   // main thread
#endif

        tm.tv_sec = 2;
        tm.tv_usec = 0; /* 15ms */
        fd_set fds;
        FD_ZERO(&fds);
        FD_SET(STDIN, &fds);
        ret = select(FD_SETSIZE, &fds, /*(fd_set *)*/ NULL, /*(fd_set *)*/ NULL, &tm);
        if (ret == -1)
        {
            perror("select");
            exit(1);
        }
        else if(ret == 0)
        {
            
        }
        //3. 判断STDIN的读属性是否发生变化
        if (FD_ISSET(STDIN, &fds))
        {
            // gets(in_buf);
            if (fgets(in_buf, 10240, stdin) != NULL)
            {
                cmd_analysis(handle, in_buf);
            }
            if(strstr(in_buf, "exit"))
            {
                break;
            }
        }
    }
#ifdef  DISPOSE_THREAD   
        thread_handle->ThreadFlag = 0;
#else
    free(thread_handle->RecBuff);
    free(thread_handle);
#endif
    free(in_buf);
    return 0;
    //if(argc>1)
}