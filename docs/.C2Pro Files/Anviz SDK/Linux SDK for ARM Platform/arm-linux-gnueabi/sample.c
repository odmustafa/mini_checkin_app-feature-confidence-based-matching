#include "crosschex.h"
#include <stdio.h>
#if 0
int main()
{
    printf("~~~~~~~~~~~~~");
    CChex_Init();
    printf("~~~~~~~~~~~~~");
    void * temp  = CChex_Start();
    if(temp)
    {
    printf("%p~~~~~~~~~~~~~",temp);
    CCHex_ClientConnect(temp,"192.168.19.151", 5010);

    printf("%d\n",CChex_Version());

    printf("%d\n",CChex_Get_Service_Port(temp));
    }
    sleep(10000);
    return 0;
}
#else

int main(int argc, const char *argv[])
{
    int devidx;
    int type;
    int ret;
    int len;
    char buff[1024];
    char tmp_prt[2048];
    int p_port = 5010;

    static int test = 0xff; //no test


    char Ip[16] = "192.168.19.75";

    if(argc == 3)
    {
        p_port = atoi(argv[2]);
        if(p_port <0 || p_port >65535)
        {
            printf("Port : 0--65535 \n");
            return 0;
        }
        printf("~~~~~~~~~%s\n",argv[1]);
        strncpy(Ip,argv[1],15);
        
    }
    else
    {
        printf("example: a.out 192.168.100.100 5010\n ");
        return 0;
    }
    //static int test = 0;  //test dlemployee
    //static int test = 20;  //test reboot
    //static int test = 0x30;  //test settime
    //static int test = 0x40; //test get all head
    //static int test = 0x50; //test get net config
    CChex_Init();

    void *handle = CChex_Start();

    printf("Ver: %d", CChex_Version());
    
    CCHex_ClientConnect(handle,Ip,p_port);
    printf("~~~~~~~~~~~~~~~~while\n");
    while (handle)
    {
        ret = CChex_Update(handle, &devidx, &type, buff, 1024);
        printf("~~~~~~~~~~~~~~~~ret = %d  \n",ret);
        if (ret > 0)
        {
            switch (type)
            {
            case CCHEX_RET_RECORD_INFO_TYPE:
            {
                CCHEX_RET_RECORD_INFO_STRU *info = (CCHEX_RET_RECORD_INFO_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, Date:%8x, BackId:%d, RecType:%d, WorkType:%d, NewRecord:%d\n", info->MachineId, info->EmployeeId[0], info->EmployeeId[1], info->EmployeeId[2], info->EmployeeId[3], info->EmployeeId[4], *(unsigned int *)(info->Date), info->BackId, info->RecordType, info->WorkType[2] * 0x10000 + info->WorkType[1] * 0x100 + info->WorkType[0], info->NewRecordFlag);
                // //DBG_INFO("%s", tmp_prt);
                // //write_log(tmp_prt, len);

                //test
                if (0 == test)
                {
                    CChex_DownloadEmployeeInfo(handle, devidx, 10);
                    //test = 1;   //test upload employee
                    test = 10; //test download fp
                }
                if (0x20 == test)
                {
                    CChex_RebootDevice(handle, devidx);
                    test++;
                }
                if (0x30 == test)
                {
                    CChex_SetTime(handle, devidx, 2017, 9, 30, 10, 10, 10);
                    test++;
                }
                if (0x40 == test)
                {
                    CChex_MsgGetAllHead(handle, devidx);
                    test++;
                }
                if (0x45 == test)
                {
                    CChex_GetNetConfig(handle, devidx);
                    test++;
                }
            }
            break;

            case CCHEX_RET_DEV_LOGIN_TYPE:
            {
                CCHEX_RET_DEV_LOGIN_STRU *info = (CCHEX_RET_DEV_LOGIN_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, Version:%8s, DevType:%8s, Addr:%s Login\n", info->MachineId, info->Version, info->DevType, info->Addr);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);

                CChex_GetNetConfig(handle, devidx);
                //CChex_DownloadAllRecords(handle,devidx);
            }
            break;
            case CCHEX_RET_CLINECT_CONNECT_FAIL_TYPE:
            {
                CCHEX_RET_DEV_CONNECT_STRU *info = (CCHEX_RET_DEV_CONNECT_STRU*)buff;
                printf("Connect Fail IP = %s\n",info->Addr);
            }
            break;
            case CCHEX_RET_DEV_LOGOUT_TYPE:
            {
                CCHEX_RET_DEV_LOGOUT_STRU *info = (CCHEX_RET_DEV_LOGOUT_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, Version:%s, DevType:%s, Live:%d, Addr:%s Logout\n", info->MachineId, info->Version, info->DevType, info->Live, info->Addr);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);
            }
            break;

            case CCHEX_RET_DLFINGERPRT_TYPE:
            {
                CCHEX_RET_DLFINGERPRT_STRU *info = (CCHEX_RET_DLFINGERPRT_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, FpIdx:%d, DlFingerPrt\n", info->MachineId, info->EmployeeId[0], info->EmployeeId[1], info->EmployeeId[2], info->EmployeeId[3], info->EmployeeId[4], info->FpIdx);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);
            }
            break;
            case CCHEX_RET_ULFINGERPRT_TYPE:
            {
                CCHEX_RET_ULFINGERPRT_STRU *info = (CCHEX_RET_ULFINGERPRT_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, FpIdx:%d, UlFingerPrt\n", info->MachineId, info->EmployeeId[0], info->EmployeeId[1], info->EmployeeId[2], info->EmployeeId[3], info->EmployeeId[4], info->FpIdx);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);
            }
            break;

            case CCHEX_RET_DLEMPLOYEE_INFO_TYPE:
            {
                CCHEX_RET_DLEMPLOYEE_INFO_STRU *info = (CCHEX_RET_DLEMPLOYEE_INFO_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, Pwd:%02x%02x%02x, CardId:%d, Name:%s, DeptId:%d, GroupId:%d, Mode:%d, FpStatus:%02x%02x, Special:%d, DlEmployee\n", info->MachineId, info->Employee.EmployeeId[0], info->Employee.EmployeeId[1], info->Employee.EmployeeId[2], info->Employee.EmployeeId[3], info->Employee.EmployeeId[4], info->Employee.Passwd[0], info->Employee.Passwd[1], info->Employee.Passwd[2], info->Employee.CardId[0] + info->Employee.CardId[1] * 0x100 + info->Employee.CardId[2] * 0x10000, info->Employee.EmployeeName, info->Employee.DepartmentId, info->Employee.GroupId, info->Employee.Mode, info->Employee.FpStatus[0], info->Employee.FpStatus[1], info->Employee.Special);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);
            }
            break;
            case CCHEX_RET_DLEMPLOYEE2_INFO_TYPE:
            {
                CCHEX_RET_DLEMPLOYEE2_INFO_STRU *info = (CCHEX_RET_DLEMPLOYEE2_INFO_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, Pwd:%02x%02x%02x, CardId:%d, Name:%s, DeptId:%d, GroupId:%d, Mode:%d, FpStatus:%02x%02x, Special:%d, DlEmployee2\n", info->MachineId, info->Employee.EmployeeId[0], info->Employee.EmployeeId[1], info->Employee.EmployeeId[2], info->Employee.EmployeeId[3], info->Employee.EmployeeId[4], info->Employee.Passwd[0], info->Employee.Passwd[1], info->Employee.Passwd[2]
                               //, *(unsigned int *)info->Employee.CardId
                               ,
                               info->Employee.CardId[0] * 0x1000000 + info->Employee.CardId[1] * 0x10000 + info->Employee.CardId[2] * 0x100 + info->Employee.CardId[2], info->Employee.EmployeeName, info->Employee.DepartmentId, info->Employee.GroupId, info->Employee.Mode, info->Employee.FpStatus[0], info->Employee.FpStatus[1], info->Employee.Special);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);
            }
            break;

            case CCHEX_RET_DLEMPLOYEE2UNICODE_INFO_TYPE:
            {
                CCHEX_RET_DLEMPLOYEE2UNICODE_INFO_STRU *info = (CCHEX_RET_DLEMPLOYEE2UNICODE_INFO_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x(%u), Pwd:%02x%02x%02x, CardId:[%02x%02x%02x%02x], Name:%s, DeptId:%d, GroupId:%d, Mode:%d, FpStatus:%02x%02x, Special:%d DlEmployeeUnicode\n", info->MachineId, info->Employee.EmployeeId[0], info->Employee.EmployeeId[1], info->Employee.EmployeeId[2], info->Employee.EmployeeId[3], info->Employee.EmployeeId[4], htonl(*(unsigned int *)&info->Employee.EmployeeId[1]), info->Employee.Passwd[0], info->Employee.Passwd[1], info->Employee.Passwd[2]
                               //, *(unsigned int *)info->Employee.CardId
                               ,
                               info->Employee.CardId[0], info->Employee.CardId[1], info->Employee.CardId[2], info->Employee.CardId[3]
                               //, info->Employee.CardId[0]*0x1000000+info->Employee.CardId[1]*0x10000 + info->Employee.CardId[2]*0x100 + info->Employee.CardId[3]
                               ,
                               info->Employee.EmployeeName, info->Employee.DepartmentId, info->Employee.GroupId, info->Employee.Mode, info->Employee.FpStatus[0], info->Employee.FpStatus[1], info->Employee.Special);
                //DBG_INFO("%s", tmp_prt);
                //write_log(tmp_prt, len);

                if (1 == test)
                {
                    info->Employee.EmployeeId[2]++;
                    CChex_UploadEmployee2UnicodeInfo(handle, devidx, &info->Employee, 1);
                    test = 2;
                }
                if (10 == test)
                {
                    CChex_DownloadFingerPrint(handle, devidx, info->Employee.EmployeeId, 1);
                    test = 11;
                }
            }
            break;

            case CCHEX_RET_ULEMPLOYEE_INFO_TYPE:
            {
                //DBG_INFO("~~~~~~~~~~~~~~~~~~~~~~~~~%d", type);

                //my_debug_msg((unsigned char *)buff, ret);
            }
            break;

            case CCHEX_RET_ULEMPLOYEE2_INFO_TYPE:
            {
                //DBG_INFO("~~~~~~~~~~~~~~~~~~~~~~~~~%d", type);
            }
            break;

            case CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_TYPE:
            {
                CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_STRU *info = (CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EpId:%02x%02x%02x%02x%02x, Pwd:%02x%02x%02x, CardId:[%02x%02x%02x%02x], Name:%s, DeptId:%d, GroupId:%d, Mode:%d, FpStatus:%02x%02x, Special:%d result:%d, UlEmployeeUnicode\n", info->MachineId, info->Employee.EmployeeId[0], info->Employee.EmployeeId[1], info->Employee.EmployeeId[2], info->Employee.EmployeeId[3], info->Employee.EmployeeId[4], info->Employee.Passwd[0], info->Employee.Passwd[1], info->Employee.Passwd[2]
                               //, *(unsigned int *)info->Employee.CardId
                               ,
                               info->Employee.CardId[0], info->Employee.CardId[1], info->Employee.CardId[2], info->Employee.CardId[3]
                               //, info->Employee.CardId[0]*0x1000000+info->Employee.CardId[1]*0x10000 + info->Employee.CardId[2]*0x100 + info->Employee.CardId[3]
                               ,
                               info->Employee.EmployeeName, info->Employee.DepartmentId, info->Employee.GroupId, info->Employee.Mode, info->Employee.FpStatus[0], info->Employee.FpStatus[1], info->Employee.Special, info->Result);
                //DBG_INFO("%s", tmp_prt);

                //my_debug_msg((unsigned char *)buff, ret);
            }
            break;

            case CCHEX_RET_DEV_STATUS_TYPE:
            {
                CCHEX_RET_DEV_STATUS_STRU *status = (CCHEX_RET_DEV_STATUS_STRU *)buff;
                len = snprintf(tmp_prt, 2048, "Mid:%08x, EmployeeNum:%u, FingerPrtNum:%u, PasswdNum:%u, CardNum:%u, TotalRecordNum:%u\n", status->MachineId, status->EmployeeNum, status->FingerPrtNum, status->PasswdNum, status->CardNum, status->TotalRecNum);
                //DBG_INFO("%s", tmp_prt);
            }
            break;

            case CCHEX_RET_MSGGETALLHEADUNICODE_INFO_TYPE:
            {
                ;
            }
            break;

            case CCHEX_RET_GETNETCFG_TYPE:
            {
                CCHEX_RET_GETNETCFG_STRU *status = (CCHEX_RET_GETNETCFG_STRU *)buff;
                //DBG_INFO("IP:%d.%d.%d.%d, Gw:%d.%d.%d.%d, servip:%d.%d.%d.%d, MAC[%02X %02X %02X %02X %02X %02X] ,Mode:%d ", status->Cfg.IpAddr[0], status->Cfg.IpAddr[1], status->Cfg.IpAddr[2], status->Cfg.IpAddr[3], status->Cfg.GwAddr[0], status->Cfg.GwAddr[1], status->Cfg.GwAddr[2], status->Cfg.GwAddr[3], status->Cfg.ServAddr[0], status->Cfg.ServAddr[1], status->Cfg.ServAddr[2], status->Cfg.ServAddr[3], status->Cfg.MacAddr[0], status->Cfg.MacAddr[1], status->Cfg.MacAddr[2], status->Cfg.MacAddr[3], status->Cfg.MacAddr[4], status->Cfg.MacAddr[5], status->Cfg.Mode);

                // if(1)
                // {
                //     CCHEX_NETCFG_INFO_STRU cfg;
                //     memcpy(&cfg, &status->Cfg, sizeof(CCHEX_NETCFG_INFO_STRU));
                //     cfg.ServAddr[2] = 0;
                //     cfg.ServAddr[3] = 254;
                //     CChex_SetNetConfig(handle, devidx, &cfg);
                // }
            }
            break;

            default:
                //DBG_INFO("RET:%d, type:%d", ret, type);
                break;
            }
            // print ???
        }
        else if (0 == ret)
        {
            sleep(1);
        }
        else
        {
            ; //buff to small
        }
    }

    return 0;
    //if(argc>1)
}
#endif