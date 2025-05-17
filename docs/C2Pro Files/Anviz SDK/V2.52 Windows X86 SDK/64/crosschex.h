#ifndef _CROSS_CHEX_H_
#define _CROSS_CHEX_H_

#include "myplatform.h"

#ifdef API_STATICLIB
    #define API_EXTERN
#elif defined(WIN32) || defined(_WIN32)
    #if defined(_PLAY_IMPL_EXPORT_)
        #define API_EXTERN __declspec(dllexport)
    #else
        #define API_EXTERN __declspec(dllimport)
    #endif
#elif defined(MYPLATFORM_LINUX)
    #define API_EXTERN __attribute__ ((visibility("default")))
#else
    #define API_EXTERN
#endif


#ifdef __cplusplus

extern "C" {
#endif
/****************************
 * 协议类型
 * **************************/
#define DEV_TYPE_FLAG_42                0x01
#define DEV_TYPE_FLAG_72_ASCII          0x02 //common ASCII
#define DEV_TYPE_FLAG_72_UNICODE        0x04 //common UNICODE
#define DEV_TYPE_FLAG_22                0x08
#define DEV_TYPE_FLAG_72_UNICODE_DR     0x10 // DR  has  two  name 
#define DEV_TYPE_FLAG_UNICODE_TIME      0x20 // BOLID has start time, end time
#define DEV_TYPE_VER_4_NEWID            0x40 // New has start ,time end time  and  has String Person_Id
#define DEV_TYPE_FLAG_SAC               0x80 // standard access control
#define DEV_TYPE_FLAG_CARDNO_BYTE_7     0x1000000 //for seats : cardNo lenght is 7Bit

#define DEV_TYPE_FLAG_FP_LEN_338        0x1000  //通用指纹设备
#define DEV_TYPE_FLAG_FP_LEN_1200       0x2000  //IRIS2000设备 //旧固件1200 新固件为2400
#define DEV_TYPE_FLAG_FP_LEN_2400       0x2000  //IRIS2000设备 //旧固件1200 新固件为2400
#define DEV_TYPE_FLAG_FP_LEN_2048       0x4000  //OA1000PU设备
#define DEV_TYPE_FLAG_FP_LEN_6144       0x8000  //OA1000PM
// #define DEV_TYPE_FLAG_FP_LEN_10240      0x10000 //保留机型 取消该类型
#define DEV_TYPE_FLAG_FACE_DEEP_2052    0x10000 //FDEEP5M FDEEP5TM FINGERPRINT_DATA_LEN_2052 代替 DEV_TYPE_FLAG_FP_LEN_10240 FDEEP3M FDEEP3TM FD52M FD52TM FK20M FK20TM  PASS7PM PASS7PTM 
#define DEV_TYPE_FLAG_FACEPASS_15360    0x20000 //FACE7
#define DEV_TYPE_FLAG_FACEPASS_2056     0x40000 //FACE7FAI
#define DEV_TYPE_FLAG_FACEPASS_1028     0x80000 //FDEEP5 FDEEP5T FDEEP3 FDEEP3T FD52 FACE7PRO FK20 PASS7PT

#define DEV_TYPE_FLAG_FACEPASS_PICTURE  0x800000 //支持人脸图片数据模块 (FDEEP5 FDEEP5T FDEEP5M FDEEP5TM,FDEEP3 FDEEP3T FDEEP3M FDEEP3TM FD52 FD52M FD52TM FACE7PRO FK20 FK20M FK20TM)

#define DEV_TYPE_FLAG_RECORD_COMMON         0x100000 //通用记录
#define DEV_TYPE_FLAG_RECORD_TM             0x200000 //测温或口罩记录 (FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM  FDEEP3T, FDEEP3M, FDEEP3TM ,PASS7PT PASS7PM PASS7PTM FD52 FD52M FD52TM FK20 FK20M FK20TM)
#define DEV_TYPE_FLAG_RECORD_TEMPERATURE_T  0x400000 //支持获取测温记录图片 (FACE7TM  FACE7T FDEEP5T FDEEP5TM FDEEP3T FDEEP3TM PASS7PT PASS7PTM FD52TM FK20TM)


#define MAX_EMPLOYEE_ID_VER_4_NEWID       28        //员工号为字符串
#define MAX_EMPLOYEE_ID             5               //员工号为数字
#define MAX_EMPLOYEE_NAME_ASCII     10
#define MAX_EMPLOYEE_NAME_UNICODE   20
#define MAX_EMPLOYEE_NAME_22        64
#define MAX_EMPLOYEE_NAME_DR        160
#define MAX_PWD_LEN                 3
#define MAC_CARD_ID_SMALL_LEN       3
#define MAC_CARD_ID_LEN             4
#define MAC_CARD_ID_LEN_7           7
#define MAX_FRC_DR                  13
#define MAX_CURP_DR                 18
#define FP_STATUS_LEN               2
#define FINGERPRINT_DATA_LEN_15360  15360
#define FINGERPRINT_DATA_LEN_10240  10240
#define FINGERPRINT_DATA_LEN_6144   6144 // normal:338,OA1000PM:2048*3,OA1000P:2048
#define FINGERPRINT_DATA_LEN_2056   2056
#define FINGERPRINT_DATA_LEN_2052   2052
#define FINGERPRINT_DATA_LEN_2048   2048
#define FINGERPRINT_DATA_LEN_2400   2400//IRIS2000设备 原始固件1200 新固件为2400
#define FINGERPRINT_DATA_LEN_1200   1200//IRIS2000设备 原始固件1200 新固件为2400
#define FINGERPRINT_DATA_LEN_1028   1028
#define FINGERPRINT_DATA_LEN_338    338
#define SOFTWARE_VERSION_LEN        8
#define DEV_TYPE_LEN                8
#define ADDR_LEN                    24
#define DEV_TYPE_NUMBER             10
#define DEV_SERIAL_NUMBER           16



enum
{
    CCHEX_RET_RECORD_INFO_TYPE                  = 1,
    CCHEX_RET_DEV_LOGIN_TYPE                    = 2,
    CCHEX_RET_DEV_LOGOUT_TYPE                   = 3,
    CCHEX_RET_DLFINGERPRT_TYPE                  = 4,
    CCHEX_RET_ULFINGERPRT_TYPE                  = 5,

    CCHEX_RET_ULEMPLOYEE_INFO_TYPE              = 6,
    CCHEX_RET_ULEMPLOYEE2_INFO_TYPE             = 7,
    CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_TYPE      = 8,

    CCHEX_RET_DLEMPLOYEE_INFO_TYPE              = 9,
    CCHEX_RET_DLEMPLOYEE2_INFO_TYPE             = 10,
    CCHEX_RET_DLEMPLOYEE2UNICODE_INFO_TYPE      = 11,

    CCHEX_RET_MSGGETBYIDX_INFO_TYPE             = 12,
    CCHEX_RET_MSGGETBYIDX_UNICODE_INFO_TYPE     = 13,
    CCHEX_RET_MSGADDNEW_INFO_TYPE               = 14,
    CCHEX_RET_MSGADDNEW_UNICODE_INFO_TYPE       = 15,
    CCHEX_RET_MSGDELBYIDX_INFO_TYPE             = 16,
    CCHEX_RET_MSGGETALLHEAD_INFO_TYPE           = 17,

    CCHEX_RET_REBOOT_TYPE                       = 18,
    CCHEX_RET_DEV_STATUS_TYPE                   = 19,
    CCHEX_RET_MSGGETALLHEADUNICODE_INFO_TYPE    = 20,
    CCHEX_RET_SETTIME_TYPE                      = 21,
    CCHEX_RET_UPLOADFILE_TYPE                   = 22,
    CCHEX_RET_GETNETCFG_TYPE                    = 23,
    CCHEX_RET_SETNETCFG_TYPE                    = 24,
    CCHEX_RET_GET_SN_TYPE                       = 25,
    CCHEX_RET_SET_SN_TYPE                       = 26,
    CCHEX_RET_DLEMPLOYEE_3_TYPE                 = 27, // 761
    CCHEX_RET_ULEMPLOYEE_3_TYPE                 = 28, // 761
    CCHEX_RET_GET_BASIC_CFG_TYPE                = 29,
    CCHEX_RET_SET_BASIC_CFG_TYPE                = 30,
    CCHEX_RET_DEL_EMPLOYEE_INFO_TYPE            = 31,
    CCHEX_RET_DLEMPLOYEE2UNICODE_DR_INFO_TYPE   = 32,
    CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE      = 33,
    CCHEX_RET_MSGGETBYIDX_UNICODE_S_DATE_INFO_TYPE  = 34,       //for Seats
    CCHEX_RET_MSGADDNEW_UNICODE_S_DATE_INFO_TYPE    = 35,         //for Seats
    CCHEX_RET_MSGGETALLHEADUNICODE_S_DATE_INFO_TYPE = 36,       //for Seats

    CCHEX_RET_GET_BASIC_CFG2_TYPE               = 37,
    CCHEX_RET_SET_BASIC_CFG2_TYPE               = 38,
    CCHEX_RET_GETTIME_TYPE                      = 39,
    CCHEX_RET_INIT_USER_AREA_TYPE               = 40,
    CCHEX_RET_INIT_SYSTEM_TYPE                  = 41,
    CCHEX_RET_GET_PERIOD_TIME_TYPE              = 42,
    CCHEX_RET_SET_PERIOD_TIME_TYPE              = 43,
    CCHEX_RET_GET_TEAM_INFO_TYPE                = 44,
    CCHEX_RET_SET_TEAM_INFO_TYPE                = 45,
    CCHEX_RET_ADD_FINGERPRINT_ONLINE_TYPE       = 46,
    CCHEX_RET_FORCED_UNLOCK_TYPE                = 47,
    CCHEX_RET_UDP_SEARCH_DEV_TYPE               = 48,
    CCHEX_RET_UDP_SET_DEV_CONFIG_TYPE           = 49,

    CCHEX_RET_GET_INFOMATION_CODE_TYPE          = 50,
    CCHEX_RET_SET_INFOMATION_CODE_TYPE          = 51,
    CCHEX_RET_GET_BELL_INFO_TYPE                = 52,
    CCHEX_RET_SET_BELL_INFO_TYPE                = 53,
    CCHEX_RET_LIVE_SEND_ATTENDANCE_TYPE         = 54,
    CCHEX_RET_GET_USER_ATTENDANCE_STATUS_TYPE   = 55,
    CCHEX_RET_SET_USER_ATTENDANCE_STATUS_TYPE   = 56,
    CCHEX_RET_CLEAR_ADMINISTRAT_FLAG_TYPE       = 57,
    CCHEX_RET_GET_SPECIAL_STATUS_TYPE           = 58,
    CCHEX_RET_GET_ADMIN_CARD_PWD_TYPE           = 59,
    CCHEX_RET_SET_ADMIN_CARD_PWD_TYPE           = 60,
    CCHEX_RET_GET_DST_PARAM_TYPE                = 61,
    CCHEX_RET_SET_DST_PARAM_TYPE                = 62,
    CCHEX_RET_GET_DEV_EXT_INFO_TYPE             = 63,
    CCHEX_RET_SET_DEV_EXT_INFO_TYPE             = 64,
    CCHEX_RET_GET_BASIC_CFG3_TYPE               = 65,
    CCHEX_RET_SET_BASIC_CFG3_TYPE               = 66,
    CCHEX_RET_CONNECTION_AUTHENTICATION_TYPE    = 67,
    CCHEX_RET_GET_RECORD_NUMBER_TYPE            = 68,
    CCHEX_RET_GET_RECORD_BY_EMPLOYEE_TIME_TYPE  = 69,

    CCHEX_RET_GET_RECORD_INFO_STATUS_TYPE       = 70,
    CCHEX_RET_GET_NEW_RECORD_INFO_TYPE          = 71,

    CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE           = 72,
    CCHEX_RET_GET_BASIC_CFG5_TYPE               = 73,
    CCHEX_RET_SET_BASIC_CFG5_TYPE               = 74,
    CCHEX_RET_GET_CARD_ID_TYPE                  = 75,
    CCHEX_RET_SET_DEV_CURRENT_STATUS_TYPE       = 76,
    CCHEX_RET_GET_URL_TYPE                      = 77,
    CCHEX_RET_SET_URL_TYPE                      = 78,
    CCHEX_RET_GET_STATUS_SWITCH_TYPE            = 79,
    CCHEX_RET_SET_STATUS_SWITCH_TYPE            = 80,
    CCHEX_RET_GET_STATUS_SWITCH_EXT_TYPE        = 81,
    CCHEX_RET_SET_STATUS_SWITCH_EXT_TYPE        = 82,
    CCHEX_RET_UPDATEFILE_STATUS_TYPE            = 83,

    CCHEX_RET_GET_MACHINE_ID_TYPE               = 84,
    CCHEX_RET_SET_MACHINE_ID_TYPE               = 85,
    CCHEX_RET_GET_MACHINE_TYPE_TYPE             = 86,

    CCHEX_RET_UPLOAD_RECORD_TYPE                = 87,
    CCHEX_RET_GET_ONE_EMPLOYEE_INFO_TYPE        = 88, 
    CCHEX_RET_ULEMPLOYEE_VER_4_NEWID_TYPE       = 89,
    CCHEX_RET_MANAGE_LOG_RECORD_TYPE            = 90,

    CCHEX_RET_PICTURE_GET_TOTAL_NUMBER_TYPE     = 91,
    CCHEX_RET_PICTURE_GET_ALL_HEAD_TYPE         = 92,
    CCHEX_RET_PICTURE_GET_DATA_BY_EID_TIME_TYPE = 93,
    CCHEX_RET_PICTURE_DEL_DATA_BY_EID_TIME_TYPE = 94,
    CCHEX_RET_LIVE_SEND_SPECIAL_STATUS_TYPE     = 95,
    
    CCHEX_RET_TM_ALL_RECORD_INFO_TYPE           = 150,              //CCHEX_RET_TM_RECORD_INFO_STRU
    CCHEX_RET_TM_NEW_RECORD_INFO_TYPE           = 151,              //CCHEX_RET_TM_RECORD_INFO_STRU
    CCHEX_RET_TM_LIVE_SEND_RECORD_INFO_TYPE     = 152,              //CCHEX_RET_TM_LIVE_SEND_RECORD_INFO_STRU
    CCHEX_RET_TM_UPLOAD_RECORD_INFO_TYPE        = 153,              //CCHEX_RET_TM_UPLOAD_RECORD_STRU
    CCHEX_RET_TM_RECORD_BY_EMPLOYEE_TIME_TYPE   = 154,              //CCHEX_RET_TM_RECORD_INFO_STRU

    CCHEX_RET_GET_T_RECORD_NUMBER_TYPE          = 155,              //CCHEX_RET_GET_T_RECORD_NUMBER_STRU
    CCHEX_RET_GET_T_RECORD_TYPE                 = 156,              //CCHEX_RET_GET_T_RECORD_STRU
    CCHEX_RET_GET_T_PICTURE_BY_RECORD_ID_TYPE   = 157,              //CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU
    CCHEX_RET_DEL_T_PICTURE_BY_RECORD_ID_TYPE   = 158,              //CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU

    CCHEX_RET_DOWNLOAD_FACE_PICTURE_MODULE_TYPE = 159,              //CCHEX_DOWNLOAD_FACE_PICTURE_MODULE
    CCHEX_RET_UPLOAD_FACE_PICTURE_MODULE_TYPE   = 160,              //CCHEX_UPLOAD_FACE_PICTURE_MODULE
    CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE   = 161,          //CCHEX_DOWNLOAD_FACE_PICTURE_MODULE

    CCHEX_RET_CLINECT_CONNECT_FAIL_TYPE         = 200,

    CCHEX_RET_DEV_LOGIN_CHANGE_TYPE             = 201,

    CCHEX_RET_RECORD_INFO_CARD_BYTE7_TYPE       = 251,

//SAC 门禁控制器 开始
    CCHEX_SAC_DOWNLOAD_COMMON_TYPE              = 401,
    CCHEX_SAC_UPLOAD_COMMON_TYPE                = 402,
    CCHEX_SAC_DELETE_COMMON_TYPE                = 403,
    CCHEX_SAC_INIT_COMMON_TYPE                  = 404,
    CCHEX_SAC_PUSH_COMMON_TYPE                  = 405,
//SAC 门禁控制器 结束
};

// enum
// {
//     CCHEX_SAC_DEV_LOGIN_TYPE                        = 1,
//     CCHEX_SAC_RET_DEV_LOGOUT_TYPE                   = 2,
// };

/*******************************************************************************

*******************************************************************************/
//client by IP
typedef struct
{
    int Result; //0 ok, -1 err
    char Addr[ADDR_LEN];    //IP地址
} CCHEX_RET_DEV_CONNECT_STRU;   //连接失败返回

//get time
typedef struct
{
    unsigned int Year;      //年
    unsigned int Month;     //月
    unsigned int Day;       //日
    unsigned int Hour;      //时
    unsigned int Min;       //分
    unsigned int Sec;       //秒
}CCHEX_MSG_GETTIME_STRU;    //时间结构体24 bytes
typedef struct
{
    unsigned int MachineId;         //机器号
    int Result; //0 ok, -1 err      //执行结果
    CCHEX_MSG_GETTIME_STRU config;  //时间信息
} CCHEX_MSG_GETTIME_STRU_EXT_INF;

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char StartYear;                    //开始年
    unsigned char StartMonth;                   //开始月
    unsigned char StartDay;                     //开始日

    unsigned char EndYear;                      //结束年
    unsigned char EndMonth;                     //结束月
    unsigned char EndDay;                       //结束日
} CCHEX_MSGHEAD_INFO_STRU;                      //短信息普通版   

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //员工号
    unsigned char StartYear;                                //开始年
    unsigned char StartMonth;                               //开始月
    unsigned char StartDay;                                 //开始日

    unsigned char EndYear;                                  //结束年 
    unsigned char EndMonth;                                 //结束月
    unsigned char EndDay;                                   //结束日
    //unsigned char Padding;
} CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID;          //普通字符串员工号版

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char StartYear;
    unsigned char StartMonth;
    unsigned char StartDay;
    unsigned char StartHour;            
    unsigned char StartMin;
    unsigned char StartSec;

    unsigned char EndYear;
    unsigned char EndMonth;
    unsigned char EndDay;
    unsigned char EndHour;
    unsigned char EndMin;
    unsigned char EndSec;

} CCHEX_MSGHEADUNICODE_INFO_STRU; // len 17 , 短信息SEAtS定制普通版
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];
    unsigned char StartYear;
    unsigned char StartMonth;
    unsigned char StartDay;
    unsigned char StartHour;
    unsigned char StartMin;
    unsigned char StartSec;

    unsigned char EndYear;
    unsigned char EndMonth;
    unsigned char EndDay;
    unsigned char EndHour;
    unsigned char EndMin;
    unsigned char EndSec;

} CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID; // len 17 , 短信息SEAtS定制员工字符串版

typedef struct
{
    unsigned char IpAddr[4];        // 每一位是机器IP地址中对应的数字
    unsigned char IpMask[4];        // 每一位是子网掩码中对应的数字
    unsigned char MacAddr[6];       // 每一位是MAC中对应的数字
    unsigned char GwAddr[4];        // 每一位是网关中对应的数字
    unsigned char ServAddr[4];      // 每一位是服务器IP中对应的数字
    unsigned char RemoteEnable;     //暂时不用
    unsigned char Port[2];          // 服务器的端口号
    unsigned char Mode;             // 模式：0:服务器，1:客户端
    unsigned char DhcpEnable;       // DHCP是否打开，0:关闭，1:打开
} CCHEX_NETCFG_INFO_STRU; //27 bytes

/*******************************************************************************
    
*******************************************************************************/

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
} CCHEX_RET_COMMON_STRU;


#define CCHEX_RET_REBOOT_STRU                               CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SETTIME_STRU                              CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SETNETCFG_STRU                            CCHEX_RET_COMMON_STRU

#define CCHEX_RET_INIT_USER_AREA_STRU                       CCHEX_RET_COMMON_STRU
#define CCHEX_RET_INIT_SYSTEM_STRU                          CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_BASIC_CONFIG2_STRU                    CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_PERIOD_TIME_STRU                      CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_TEAM_INFO_STRU                        CCHEX_RET_COMMON_STRU
#define CCHEX_RET_ADD_FINGERPRINT_ONLINE_STRU               CCHEX_RET_COMMON_STRU
#define CCHEX_RET_FORCED_UNLOCK_STRU                        CCHEX_RET_COMMON_STRU
#define CCHEX_RET_UDP_SET_DEV_CONFIG_STRU                   CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_DEV_CONFIG_STRU_EXT_INF               CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_INFOMATION_CODE_STRU                  CCHEX_RET_COMMON_STRU

#define CCHEX_RET_SET_BELL_INFO_STRU                        CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_USER_ATTENDANCE_STATUS_INFO_STRU      CCHEX_RET_COMMON_STRU
#define CCHEX_RET_CLEAR_ADMINISTRAT_FLAG_STRU               CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_ADMIN_PWD_STRU                        CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_DST_PARAM_STRU                        CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_DEV_EXT_INFO_STRU                     CCHEX_RET_COMMON_STRU
#define CCHEX_RET_SET_BASIC_CONFIG3_STRU                    CCHEX_RET_COMMON_STRU
#define CCHEX_RET_CONNECTION_AUTHENTICATION_STRU            CCHEX_RET_COMMON_STRU




typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    int Len;                //信息长度
    char MsgDate[];         //信息内容    CCHEX_MSGHEAD_INFO_STRU CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID + (ASCII Data)
                            //CCHEX_MSGHEADUNICODE_INFO_STRU CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID + (UNICODE Data)
} CCHEX_RET_MSGGETBYIDX_UNICODE_STRU;

typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    int Len;                //信息长度
    char MsgDate[];         //信息内容    CCHEX_MSGHEAD_INFO_STRU CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID  + (ASCII Data)
                            //CCHEX_MSGHEADUNICODE_INFO_STRU CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID + (UNICODE Data)
} CCHEX_RET_MSGADDNEW_UNICODE_STRU;

typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    int Len;                //信息长度
    char MsgDate[];         //信息内容    CCHEX_MSGHEAD_INFO_STRU[]   CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID[]数组 
                            //CCHEX_MSGHEADUNICODE_INFO_STRU[] CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID[]数组
} CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU;

typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    unsigned char Idx;      //信息索引
} CCHEX_RET_MSGDELBYIDX_UNICODE_STRU;

typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    unsigned int TotalBytes;//上传总大小
    unsigned int SendBytes; //已上传大小
} CCHEX_RET_UPLOADFILE_STRU;

typedef struct
{
    unsigned int MachineId;     //机器号
    int Result;                 //0 ok, -1 err
    CCHEX_NETCFG_INFO_STRU Cfg; //网络配置
} CCHEX_RET_GETNETCFG_STRU;

#define SN_LEN 16
typedef struct
{
    unsigned int MachineId;     //机器号
    int Result;                 //0 ok, -1 err
    unsigned char sn[SN_LEN];   //SN号
} CCHEX_RET_SN_STRU;

/*******************************************************************************
UDP  struct
*******************************************************************************/
typedef struct
{
    unsigned char IpAddr[4];    //IP地址
    unsigned char IpMask[4];    //子网掩码
    unsigned char GwAddr[4];    //网关地址
    unsigned char MacAddr[6];   //MAC
    unsigned char ServAddr[4];  //服务器IP
    unsigned char Port[2];      //端口
    unsigned char NetMode;      //模式：0:服务器，1:客户端
} CCHEX_DEV_NET_INFO_STRU; //25 bytes UDP搜索网络信息
typedef struct
{
    unsigned char DevType[DEV_TYPE_NUMBER];         //设备类型
    unsigned char DevSerialNum[DEV_SERIAL_NUMBER];  //SN号
    CCHEX_DEV_NET_INFO_STRU DevNetInfo;             //网络信息
    unsigned char Version[SOFTWARE_VERSION_LEN];    //版本号
    unsigned char Reserved[4];                      //保留的

}CCHEX_UDP_SEARCH_STRU; //  63   ::  0:Dev without DNS;UDP搜索基本信息
typedef struct
{
    CCHEX_UDP_SEARCH_STRU BasicSearch;              //UDP搜索基本信息
    char Dns[4];                             
    char Url[100];                              
}CCHEX_UDP_SEARCH_WITH_DNS_STRU; //  167  ::  1:Dev has DNS;

typedef struct
{
    unsigned char CardName[10];     //网卡名
    unsigned char IpAddr[4];        
    unsigned char IpMask[4];
    unsigned char GwAddr[4];
    unsigned char MacAddr[6];
}CCHEX_UDP_SEARCH_CARD_STRU;        //网卡信息
typedef struct
{
    unsigned char DevType[DEV_TYPE_NUMBER];
    unsigned char DevSerialNum[DEV_SERIAL_NUMBER];
    unsigned char ServAddr[4];
    unsigned char Port[2];
    unsigned char NetMode;
    unsigned char Version[SOFTWARE_VERSION_LEN];
    unsigned char Reserved[4];
    unsigned char CardNumber;
    CCHEX_UDP_SEARCH_CARD_STRU CardInfo[2];
}CCHEX_UDP_SEARCH_TWO_CARD_STRU;// Dev has two NetCard双网卡设备搜索信息

typedef struct
{
    union
    {
        CCHEX_UDP_SEARCH_STRU WithoutDns;
        CCHEX_UDP_SEARCH_WITH_DNS_STRU WithDns;
        CCHEX_UDP_SEARCH_TWO_CARD_STRU TwoNetCard;
    };
    unsigned char Padding;

    int Result; //0: ok -1: fail
    unsigned int MachineId;
    int DevHardwareDataLen; //=63:Dev without DNS(CCHEX_UDP_SEARCH_STRU); = 173:Dev has DNS(CCHEX_UDP_SEARCH_WITH_DNS_STRU); 
                            // = 102:Dev has two NetCard(CCHEX_UDP_SEARCH_TWO_CARD_STRU).
    
}CCHEX_UDP_SEARCH_STRU_EXT_INF;//UDP设备搜索信息综合结构体
typedef struct
{
    int DevNum;                                     //搜索设备数目
    CCHEX_UDP_SEARCH_STRU_EXT_INF dev_net_info[];   //所有信息数据
}CCHEX_UDP_SEARCH_ALL_STRU_EXT_INF;//4+DevNum*sizeof(CCHEX_UDP_SEARCH_STRU_EXT_INF)

typedef struct
{
    CCHEX_DEV_NET_INFO_STRU DevNetInfo;     //网卡基本配置
    unsigned char Padding[3];               //填充（无效数据）
    unsigned int NewMachineId;              //机器号
    unsigned char Reserved[4];              //保留
    unsigned char DevUserName[12];          //设备名
    unsigned char DevPassWord[12];          //设备密码
    int DevHardwareType;//设备类型0:Dev without DNS;1:Dev has DNS;
    char Dns[4];                                
    char Url[100];
}CCHEX_UDP_SET_DEV_CONFIG_STRU_EXT_INF;// UDP设置设备网络配置




/*******************************************************************************

*******************************************************************************/

typedef struct
{
    unsigned int MachineId; //机器号

    unsigned char NewRecordFlag;               //是否是新记录
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; //
    unsigned char Date[4];                     //日期时间
    unsigned char BackId;                      //备份号
    unsigned char RecordType;                  //记录类型

    unsigned char WorkType[3]; //工种        (ONLY use 3bytes )
    unsigned char Rsv;
    
    unsigned int CurIdx;
    unsigned int TotalCnt;
} CCHEX_RET_RECORD_INFO_STRU;
typedef struct
{
    unsigned int MachineId; //机器号

    unsigned char NewRecordFlag;                //是否是新记录
    unsigned char CardId[MAC_CARD_ID_LEN_7];    //卡号
    unsigned char Date[4];                      //日期时间
    unsigned char BackId;                       //备份号
    unsigned char RecordType;                   //记录类型

    unsigned char WorkType[3]; //工种        (ONLY use 3bytes )
    unsigned char Rsv;
    
    unsigned int CurIdx;                        //当前序号
    unsigned int TotalCnt;                      //总数目
} CCHEX_RET_RECORD_INFO_STRU_CARD_ID_LEN_7;

typedef struct
{
    unsigned int MachineId; //机器号

    unsigned char NewRecordFlag;               //是否是新记录
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID]; //员工号
    unsigned char Date[4];                     //日期时间
    unsigned char BackId;                      //备份号
    unsigned char RecordType;                  //记录类型

    unsigned char WorkType[3]; //工种        (ONLY use 3bytes )
    unsigned char Rsv;
    
    unsigned int CurIdx;                        //当前序号
    unsigned int TotalCnt;                      //总数目
} CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID;
/***********************************************************************
 * FACE7TM测温记录获取
 * 温度：
	三位数的整形，368表示36.8摄氏度
 * 是否戴口罩：
	0未戴口罩，1戴口罩
 * 开门类型：
	00000000开门，
	00000001温度异常未开门，
	00000010未戴口罩未开门，
	00000100门禁权限未开门,
	00000111 温度异常/未戴口罩/无门禁权限 未开门
 **********************************************************************/
typedef struct
{
    unsigned int MachineId; //机器号
    int Result; //0 ok, -1 err
    unsigned char NewRecordFlag;               //是否是新记录
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; //
    unsigned char Date[4];                     //日期时间
    unsigned char BackId;                      //备份号
    unsigned char RecordType;                  //记录类型

    unsigned char WorkType[3];                  //工种        (ONLY use 3bytes )
    unsigned char Temperature[2];               //温度整数/10  368 :36.8
    unsigned char IsMask;                       //是否戴口罩
    unsigned char OpenType;                     //开门类型
    
    unsigned int CurIdx;
    unsigned int TotalCnt;
} CCHEX_RET_TM_RECORD_INFO_STRU;
/********************************
 * FACE7TM实时推送测温记录
 *******************************/
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];
    unsigned char timestamp[4];                 //日期时间为相距2000年后的秒数2000.1.2
    unsigned char backup;                       //备分号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种
    unsigned char Temperature[2];               //温度整数/10  368 :36.8
    unsigned char IsMask;                       //是否戴口罩
    unsigned char OpenType;                     //开门类型
} CCHEX_RET_TM_LIVE_SEND_RECORD_INFO_STRU;

/********************************
 * FACE7TM上传测温记录
 *******************************/
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char timestamp[4];                 //日期时间为相距2000年后的秒数2000.1.2
    unsigned char backup;                       //备分号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种
    unsigned char Temperature[2];               //温度整数/10  368 :36.8
    unsigned char IsMask;                       //是否戴口罩
    unsigned char OpenType;                     //开门类型
} CCHEX_TM_UPLOAD_RECORD_INFO_STRU;

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_SMALL_LEN];        //卡号 
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_ASCII];//员工名

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，0~9:fp; 10:face; 11:iris1; 12:iris2             
    unsigned char Special;                              //特殊信息 位6：是否普通员工，位7：是否管理员
    unsigned char Padding;                              //无效数据
} CCHEX_EMPLOYEE_INFO_STRU; //total 27 bytes +1 padding

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN];              //卡号
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_ASCII];//员工名 ASCII编码

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，0~9:fp; 10:face; 11:iris1; 12:iris2       
    unsigned char PwdH8bit;                         //高为密码暂未使用
    unsigned char Rserved;                          //保留
    unsigned char Special;                          //特殊信息 位6：是否普通员工，位7：是否管理员
    unsigned char Padding[2];                       //无效数据
} CCHEX_EMPLOYEE2_INFO_STRU; //

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN];              //卡号
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE];//员工名 UNICODE编码

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，位0~9:fp; 10:face; 11:iris1; 12:iris2     
    unsigned char PwdH8bit;                             //高为密码暂未使用
    unsigned char Rserved;                              //保留
    unsigned char Special;                              //特殊信息 位6：是否普通员工，位7：是否管理员
} CCHEX_EMPLOYEE2UNICODE_INFO_STRU; //total 40 bytes   // UNICODE普通版
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN_7];            //卡号    
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE]; //员工名 UNICODE编码

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，位0~9:fp; 10:face; 11:iris1; 12:iris2     
    unsigned char PwdH8bit;                             //高为密码暂未使用
    unsigned char Rserved;                              //保留
    unsigned char Special;                              //特殊信息 位6：是否普通员工，位7：是否管理员
} CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU;  // for cardid len 7 卡号7位定制

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN];              //卡号    
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE];  //员工名 UNICODE编码

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，位0~9:fp; 10:face; 11:iris1; 12:iris2     
    unsigned char PwdH8bit;                             //高为密码暂未使用
    unsigned char Rserved;                              //保留
    unsigned char Special;                              //特殊信息 位6：是否普通员工，位7：是否管理员
    unsigned char start_date[4];                //this sec  begin year is 2000.1.2有效开始时间
    unsigned char end_date[4];                  //this sec  begin year is 2000.1.2有效结束时间
}CCHEX_EMPLOYEE2W2_INFO_STRU;                   //带限时信息

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //Ver 4 EmployeeId 字符串员工号
    unsigned char Passwd[MAX_PWD_LEN];                      //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN];                  //卡号    
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE];  //unicode name员工名 UNICODE编码

    unsigned char DepartmentId;                         //部门ID
    unsigned char GroupId;                              //组号
    unsigned char Mode;                                 //考勤模式
    unsigned char FpStatus[FP_STATUS_LEN]; // 指纹登记情况，位0~9:fp; 10:face; 11:iris1; 12:iris2     
    unsigned char PwdH8bit;                             //高为密码暂未使用
    unsigned char Rserved;                              //保留
    unsigned char Special;                              //特殊信息 位6：是否普通员工，位7：是否管理员
    unsigned char start_date[4];                //this sec  begin year is 2000.1.2有效开始时间
    unsigned char end_date[4];                  //this sec  begin year is 2000.1.2有效结束时间
}CCHEX_EMPLOYEE_INFO_STRU_VER_4_NEWID;                   //带限时字符串员工号信息

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];          //员工号
    unsigned char Passwd[MAX_PWD_LEN];                  //密码(char[0]>>4)密码长度，密码数字：(char[0]&0xf<<16)+(char[1]<<8)+(char[2])
    unsigned char CardId[MAC_CARD_ID_LEN];              //卡号    
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE]; //unicode name

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned char FpStatus[FP_STATUS_LEN];
    unsigned char PwdH8bit;
    unsigned char Rserved;
    unsigned char Special;
    unsigned char EmployeeName2[MAX_EMPLOYEE_NAME_DR];  //DR定制员工名
    unsigned char RFC[MAX_FRC_DR];                      //DR定制RFC
    unsigned char CURP[MAX_CURP_DR];                    //DR定制CURP
} CCHEX_EMPLOYEE2UNICODE_DR_INFO_STRU; //total 231 bytes//通用员工信息


typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];
    unsigned char Passwd[MAX_PWD_LEN];
    unsigned char CardId[MAC_CARD_ID_LEN];
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE]; //unicode name

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned char FpStatus[FP_STATUS_LEN];
    unsigned char PwdH8bit;
    unsigned char Rserved;
    unsigned char Special;
    unsigned char EmployeeName2[MAX_EMPLOYEE_NAME_DR];  //DR定制员工名
    unsigned char RFC[MAX_FRC_DR];                      //DR定制RFC
    unsigned char CURP[MAX_CURP_DR];                    //DR定制CURP
    unsigned char start_date[4];                //this sec  begin year is 2000.1.2有效开始时间
    unsigned char end_date[4];                  //this sec  begin year is 2000.1.2有效结束时间
} CCHEX_EMPLOYEE2UNICODE_DR_INFO_STRU_VER_4_NEWID; //total 231 bytes   //通用员工信息（带有效时间）

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];
    unsigned char Passwd[MAX_PWD_LEN];
    unsigned char CardId[MAC_CARD_ID_LEN];
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_22]; //unicode name

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned char FpStatus[FP_STATUS_LEN];
    unsigned char Rserved1;
    unsigned char Rserved2;
    unsigned char Special;
} CCHEX_EMPLOYEE3_INFO_STRU; //total 84 bytes   // for 761

typedef struct
{
    unsigned int MachineId;             //机器号
    int CurIdx;                         //当前序号
    int TotalCnt;                       //总数目

    CCHEX_EMPLOYEE3_INFO_STRU Employee; //员工信息
} CCHEX_RET_DLEMPLOYEE3_INFO_STRU;      //获取员工信息

// employee info for PC external interface
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; // 5 bytes,12 digitals
    unsigned char password_len;
    unsigned char max_password;     // 6 digitals
    unsigned int password;          // 3 bytes,6 digitals
    unsigned char max_card_id;      // 6 digital for 3 bytes,10 digital for 4 bytes
    unsigned int card_id;           // 3 bytes, 6 digital; 4 types 10 digitals
    unsigned char max_EmployeeName; // 10, 20 , 64
    //unsigned char is_unicode;// 0:ASCII,1:unicode
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_22];

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned int Fp_Status; // 0~9:fp; 10:face; 11:iris1; 12:iris2
    unsigned char Rserved1; // for 22
    unsigned char Rserved2; // for 72 and 22
    unsigned char Special;
    unsigned char EmployeeName2[MAX_EMPLOYEE_NAME_DR];
    unsigned char RFC[MAX_FRC_DR];
    unsigned char CURP[MAX_CURP_DR];
} CCHEX_EMPLOYEE_INFO_STRU_EXT_INF; // new 282,old total 91 bytes       DR定制员工信息

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; // 5 bytes,12 digitals
    unsigned char password_len;
    unsigned char max_password;     // 6 digitals
    unsigned int password;          // 3 bytes,6 digitals
    unsigned char max_card_id;      // 6 digital for 3 bytes,10 digital for 4 bytes
    unsigned int card_id;           // 3 bytes, 6 digital; 4 types 10 digitals
    unsigned char max_EmployeeName; // 10, 20 , 64
    //unsigned char is_unicode;// 0:ASCII,1:unicode
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_22];

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned int Fp_Status; // 0~9:fp; 10:face; 11:iris1; 12:iris2
    unsigned char Rserved1; // for 22
    unsigned char Rserved2; // for 72 and 22
    unsigned char Special;
    unsigned char EmployeeName2[MAX_EMPLOYEE_NAME_DR];
    unsigned char RFC[MAX_FRC_DR];
    unsigned char CURP[MAX_CURP_DR];
    unsigned char start_date[4];                //this sec  begin year is 2000.1.2
    unsigned char end_date[4];                  //this sec  begin year is 2000.1.2
} CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2;              

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID]; //Ver 4 EmployeeId  
    unsigned char password_len;
    unsigned char max_password;     // 6 digitals
    unsigned int password;          // 3 bytes,6 digitals
    unsigned char max_card_id;      // 6 digital for 3 bytes,10 digital for 4 bytes
    unsigned int card_id;           // 3 bytes, 6 digital; 4 types 10 digitals
    unsigned char max_EmployeeName; // 10, 20 , 64
    //unsigned char is_unicode;// 0:ASCII,1:unicode
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_22];

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned int Fp_Status; // 0~9:fp; 10:face; 11:iris1; 12:iris2
    unsigned char Rserved1; // for 22
    unsigned char Rserved2; // for 72 and 22
    unsigned char Special;
    unsigned char EmployeeName2[MAX_EMPLOYEE_NAME_DR];
    unsigned char RFC[MAX_FRC_DR];
    unsigned char CURP[MAX_CURP_DR];
    unsigned char start_date[4];                //this sec  begin year is 2000.1.2
    unsigned char end_date[4];                  //this sec  begin year is 2000.1.2
    
} CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID; 


typedef struct
{
    unsigned int MachineId;         //机器号
    int CurIdx;                     //当前序号
    int TotalCnt;                   //总数

    CCHEX_EMPLOYEE_INFO_STRU_EXT_INF Employee;  //员工信息
} CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF; // 294

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; // 5 bytes,12 digitals
    unsigned char password_len;
    unsigned char max_password;                 // 6 digitals
    unsigned int password;                      // 3 bytes,6 digitals
    unsigned char card_id[MAC_CARD_ID_LEN_7];     
    unsigned char EmployeeName[MAX_EMPLOYEE_NAME_UNICODE];

    unsigned char DepartmentId;
    unsigned char GroupId;
    unsigned char Mode;
    unsigned int Fp_Status; 
    unsigned char Rserved1; 
    unsigned char Rserved2; 
    unsigned char Special;
} CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7;
typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7 Employee;
} CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7;    //7byte卡号员工信息
typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 Employee;
} CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_W2;        //带限时的员工信息

typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID Employee;
} CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID; //带限时的字符串员工号员工信息

typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE_INFO_STRU Employee;
} CCHEX_RET_DLEMPLOYEE_INFO_STRU;                       //ASCII员工名员工信息类型1
typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE2_INFO_STRU Employee;
} CCHEX_RET_DLEMPLOYEE2_INFO_STRU;                      //ASCII员工名员工信息类型2

typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE2UNICODE_INFO_STRU Employee;
} CCHEX_RET_DLEMPLOYEE2UNICODE_INFO_STRU;

typedef struct
{
    unsigned int MachineId;
    int CurIdx;
    int TotalCnt;

    CCHEX_EMPLOYEE2UNICODE_DR_INFO_STRU Employee;
} CCHEX_RET_DLEMPLOYEE2UNICODE_DR_INFO_STRU;

typedef struct
{
    unsigned int MachineId;
    int Result; //0: ok -1: fail

    CCHEX_EMPLOYEE_INFO_STRU Employee;
} CCHEX_RET_ULEMPLOYEE_INFO_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result; //0: ok -1: fail

    CCHEX_EMPLOYEE2_INFO_STRU Employee;
} CCHEX_RET_ULEMPLOYEE2_INFO_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result; //0: ok -1: fail
    CCHEX_EMPLOYEE2UNICODE_INFO_STRU Employee;
} CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result; //0: ok -1: fail
    CCHEX_EMPLOYEE2UNICODE_DR_INFO_STRU Employee;
} CCHEX_RET_ULEMPLOYEE2UNICODE_DR_INFO_STRU;

typedef struct
{
    unsigned int MachineId;                                 //机器号
    int Result; //0 ok, -1 err          
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //员工号
    unsigned char FpIdx;                                    //指纹索引
    unsigned int fp_len;                                    //指纹长度
    unsigned char Data[FINGERPRINT_DATA_LEN_15360];         //指纹信息
} CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID;                   //下载指纹(字符串员工号)
typedef struct
{
    unsigned int MachineId;                                 //机器号
    int Result; //0 ok, -1 err  
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];              //员工号
    unsigned char FpIdx;                                    //指纹索引
    unsigned int fp_len;                                    //指纹长度
    unsigned char Data[FINGERPRINT_DATA_LEN_15360];         //指纹信息
} CCHEX_RET_DLFINGERPRT_STRU;                               //下载指纹
#define CCHEX_RET_ULFINGERPRT_STRU CCHEX_RET_DLFINGERPRT_STRU

typedef struct
{
    int DevIdx;                         //设备通信索引
    unsigned int MachineId;             //机器号
    char Addr[ADDR_LEN];                //设备地址
    char Version[SOFTWARE_VERSION_LEN]; //设备版本
    char DevType[DEV_TYPE_LEN];         //设备类型
    int DevTypeFlag;                    //设备通信协议类型（见：协议类型#define DEV_TYPE_FLAG——****）
} CCHEX_RET_DEV_LOGIN_STRU;             //设备连接成功信息

typedef struct
{
    int DevIdx;                         //设备通信索引
    unsigned int MachineId;             //机器号
    unsigned int Live;                  //设备连接时间
    char Addr[ADDR_LEN];                //设备地址
    char Version[SOFTWARE_VERSION_LEN]; //设备版本
    char DevType[DEV_TYPE_LEN];         //设备类型
} CCHEX_RET_DEV_LOGOUT_STRU;            //设备连接断开信息

typedef struct
{
    unsigned int MachineId;             //机器号

    unsigned int EmployeeNum;           //员工数
    unsigned int FingerPrtNum;          //指纹数
    unsigned int PasswdNum;             //密码数
    unsigned int CardNum;               //卡号数
    unsigned int TotalRecNum;           //总打卡记录数
    unsigned int NewRecNum;             //新打卡记录数
} CCHEX_RET_DEV_STATUS_STRU;            //设备信息概况

typedef struct
{
    unsigned int MachineId;             //机器号
    int Result; //0 ok, -1 err  
    char DevType[DEV_TYPE_LEN];         //设备类型
    int DevTypeFlag;                    //设备通信协议类型
} CCHEX_RET_GET_MACHINE_TYPE_STRU;      //获取设备类型

// basic config info for PC external interface
typedef struct
{
    unsigned char software_version[SOFTWARE_VERSION_LEN];   //设备版本号
    unsigned int password;                                  //设备密码
    unsigned char delay_for_sleep;          //休眠时延，0~250分钟，0不休眠
    unsigned char volume;                   //音量，0~5，0：静音，5：最大
    unsigned char language;                 //语种，0：中简，1：中繁，2：英，3：法，4：西，5：葡
    unsigned char date_format;              //日期格式，0:中，1：英，2：美
    unsigned char time_format;              //时间格式，0：24小时，1：12小时
    unsigned char machine_status;           //考勤状态，0~15
    unsigned char modify_language;          //修改语言，0x10可以修改设备的语言
    unsigned char cmd_version;              //指令版本
} CCHEX_GET_BASIC_CFG_INFO_STRU_EXT_INF; // 20 bytes 获取密码

typedef struct
{
    unsigned int password;          //设备密码
    unsigned char pwd_len;          //密码长度
    unsigned char delay_for_sleep;  //休眠时延，0~250分钟，0不休眠
    unsigned char volume;           //音量，0~5，0：静音，5：最大
    unsigned char language;         //语种，0：中简，1：中繁，2：英，3：法，4：西，5：葡
    unsigned char date_format;      //日期格式，0:中，1：英，2：美
    unsigned char time_format;      //时间格式，0：24小时，1：12小时
    unsigned char machine_status;   //考勤状态，0~15
    unsigned char modify_language;  //修改语言，0x10可以修改设备的语言
    unsigned char reserved;         //指令版本
} CCHEX_SET_BASIC_CFG_INFO_STRU_EXT_INF; // 设置基本配置

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    CCHEX_GET_BASIC_CFG_INFO_STRU_EXT_INF config;
} CCHEX_RET_GET_BASIC_CFG_STRU_EXT_INF;

// basic config 2 info for PC external interface
typedef struct
{
    unsigned char compare_level;            //比对精度
    unsigned char wiegand_range;            //固定韦根头
    unsigned char wiegand_type;             //韦根选项
    unsigned char work_code;                //工作码允许
    unsigned char real_time_send;           //实时发送允许 第0位：实时发送允许 0-不允许 1-允许;第1位：非法刷卡记录发送允许 0-禁用 1-允许
    unsigned char auto_update;              //智能更新允许
    unsigned char bell_lock;                //打铃允许
    unsigned char lock_delay;               //锁时延(0:不开锁，1-15：)
    unsigned int record_over_alarm;         //记录溢出警告
    unsigned char re_attendance_delay;      //重复考勤时延
    unsigned char door_sensor_alarm;        //门磁报警时延
    unsigned char bell_delay;               //打铃时延
    unsigned char correct_time;             //时间校准值
} CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF; // 16 bytes

#define CCHEX_SET_BASIC_CFG_INFO2_STRU_EXT_INF CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF config;
} CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF;

//Period of time
typedef struct
{
    unsigned char StartHour;
    unsigned char StartMin;
    unsigned char EndHour;
    unsigned char EndMin;
}CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF; //4
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF day_week[7];
}CCHEX_GET_PERIOD_TIME_STRU_EXT_INF; //4+4+28 = 36
typedef struct
{
    unsigned char SerialNumbe;
    CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF day_week[7];
}CCHEX_SET_PERIOD_TIME_STRU_EXT_INF; //29

//Team Info
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char PeriodTimeNumber[4];   
}CCHEX_GET_TEAM_INFO_STRU_EXT_INF;
typedef struct
{
    unsigned char TeamNumbe;
    unsigned char PeriodTimeNumber[4];
}CCHEX_SET_TEAM_INFO_STRU_EXT_INF;

//forced unlock
typedef struct
{
    unsigned char LockCmd;
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];
}CCHEX_FORCED_UNLOCK_STRU_EXT_INF;
// del employee for PC interface
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; // 员工号
    unsigned char backup;                      // 模板备份位置 指纹 1-10  人脸 虹膜设备无需设置
} CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF; // 6 bytes
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID]; // 28 bytes to save
    unsigned char backup;
} CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID; // 29 bytes


#define CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF
#define CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF_ID_VER_4_NEWID CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID
/*******************************************
 * 图片人脸模板
*******************************************/
#define CCHEX_FACE_PICTURE_MODULE_STRU CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF

typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char Backup;                       //备份号
    unsigned char Padding[2];                   //填充(格式对齐)
    int DataLen;                                //图片人脸模块数据长度
    char Data[];                                //图片人脸模块数据
} CCHEX_DOWNLOAD_FACE_PICTURE_MODULE; 
typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char Backup;                       //备份号
    unsigned char Padding[2];                   //填充(格式对齐)
} CCHEX_UPLOAD_FACE_PICTURE_MODULE; 



typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  // 5 bytes to save
    unsigned char padding[3];                   //
} CCHEX_RET_DEL_EMPLOYEE_INFO_STRU;             // 16 bytes

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  // 
} CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID;             // 36 bytes

#define CCHEX_RET_COMMON_WITH_EMPLOYEE_ID CCHEX_RET_DEL_EMPLOYEE_INFO_STRU
#define CCHEX_RET_COMMON__WITH_EMPLOYEE_VER_4_NEWID CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID
#define CCHEX_RET_TM_UPLOAD_RECORD_STRU CCHEX_RET_DEL_EMPLOYEE_INFO_STRU
// del record or new flag
typedef struct
{
    unsigned char del_type;
    unsigned int del_count;
} CCHEX_DEL_RECORD_OR_NEW_FLAG_INFO_STRU_EXT_INF; // 4 bytes

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned int deleted_count;
} CCHEX_RET_DEL_RECORD_OR_NEW_FLAG_STRU;

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned int fp_len;                        //ANSI VERSION   fp_len == 10   UNICODE VERSION   fp_len == 20
    unsigned char info_code[20];                //infomation code
} CCHEX_RET_GET_INFOMATOIN_CODE_STRU;           //

typedef struct
{
    unsigned char hour;
    unsigned char minute;
    unsigned char flag_week;        //星期标志flag_week(用二进制0000000分别表示：六五四三二一日) 位0-6：0-6：星期日/一/二/三/四/五/六
} CCHEX_RET_GET_BELL_TIME_POINT;
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    CCHEX_RET_GET_BELL_TIME_POINT time_point[30];
    unsigned char padding[2];
} CCHEX_RET_GET_BELL_INFO_STRU;

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];
    unsigned char timestamp[4];                 //日期时间为相距2000年后的秒数2000.1.2
    unsigned char backup;                       //备分号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种
} CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU;

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];
    unsigned char timestamp[4];                 //日期时间为相距2000年后的秒数2000.1.2
    unsigned char backup;                       //备分号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种
} CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU_VER_4_NEWID;

typedef struct
{
    unsigned int fp_len;                        //data_info :: ANSI VERSION   fp_len = 80  UNICODE VERSION   fp_len = 160
    unsigned char atten_status_number;          //考勤状态数目 8 默认
    unsigned char data_info[160];               //数据格式:8组字符串  ANSI VERSION: unsigned char [8][10]  UNICODE VERSION: unsigned char[8][20]  
    unsigned char padding[3];                   //对齐 无效数据
} CCHEX_SET_USER_ATTENDANCE_STATUS_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result;                                 //0 ok, -1 err
    unsigned int fp_len;                        //data_info :: ANSI VERSION   fp_len == 80  UNICODE VERSION   fp_len == 160
    unsigned char atten_status_number;          //考勤状态数目 == 8 默认
    unsigned char data_info[160];               //数据格式:  ANSI VERSION: unsigned char [8][10]  UNICODE VERSION: unsigned char[8][20] 
    unsigned char padding[3];
} CCHEX_RET_GET_USER_ATTENDANCE_STATUS_STRU;
//SPECIAL_STATUS
typedef struct
{
    unsigned int MachineId;
    int Result;
    unsigned char status;         //位1：门报警状态 0-正常状态 1-报警状态,位5：门状态 0-关闭 1-打开,位6：门磁状态 0-关闭 1-打开位,7：锁状态 0-关闭 1-打开
    unsigned char reserved[7];         //
} CCHEX_RET_GET_SPECIAL_STATUS_STRU;
#define CCHEX_RET_LIVE_SEND_SPECIAL_STATUS_STRU CCHEX_RET_GET_SPECIAL_STATUS_STRU
//ADMIN_CARDNUM_PWD
typedef struct
{
    unsigned int MachineId;
    int Result;
    unsigned char data[13];
    unsigned char padding[3];
} CCHEX_RET_GET_ADMIN_CARDNUM_PWD_STRU;
//DST_PARAM
typedef struct
{
    unsigned char month;
    unsigned char day;
    unsigned char week_num;       //周次定义如下：0x01-0x04：前1-4周 0x81-0x82：后1-2周
    unsigned char flag_week;      //星期标志flag_week 0-6:(用二进制0000000分别表示：六五四三二一日)
    unsigned char hour;
    unsigned char minute;
    unsigned char sec;
} GET_DST_PARAM_TIME;   //7B
typedef struct
{
    unsigned char enabled;          //0-不启用      1-启用；
    unsigned char date_week_type;   //1-日期格式	2-星期格式；
    GET_DST_PARAM_TIME start_time;
    GET_DST_PARAM_TIME special_time;
} CCHEX_SET_DST_PARAM_STRU;         //16B
typedef struct
{
    unsigned int MachineId;
    int Result;
    CCHEX_SET_DST_PARAM_STRU param;
} CCHEX_RET_GET_DST_PARAM_STRU;

//DEV EXT INFO
typedef struct
{
    char manufacturer_name[50];         //厂商名称
    char manufacturer_addr[100];        //厂商地址
    char duty_paragraph[15];            //税号
    char reserved[155];                 //预留
} CCHEX_SET_DEV_EXT_INFO_STRU;  //320B
typedef struct
{
    unsigned int MachineId;
    int Result;
    CCHEX_SET_DEV_EXT_INFO_STRU param;
} CCHEX_RET_GET_DEV_EXT_INFO_STRU;

//basic config 3 info for PC external interface
typedef struct
{
    unsigned char wiegand_type;         //韦根读取方式
    unsigned char online_mode;
    unsigned char collect_level;
    unsigned char pwd_status;           //连接密码状态  =0 网络连接时不需要验证通讯密码 =1网络连接时需要先发送0x04命令 验证通讯密码
    unsigned char sensor_status;           //=0 不主动汇报门磁状态  =1主动汇报门磁状态（设备主动发送0x2F命令的应答包)
    unsigned char reserved[8];
    unsigned char independent_time;     
    unsigned char m5_t5_status;         //= 0	禁用 = 1	启用，本机状态为出=2	启用，本机状态为入 =4	禁用，本机状态为出 =5	禁用，本机状态为入
} CCHEX_SET_BASIC_CFG_INFO3_STRU;  //15B
typedef struct
{
    unsigned int MachineId;
    int Result;
    CCHEX_SET_BASIC_CFG_INFO3_STRU param;
    unsigned char padding;
} CCHEX_RET_GET_BASIC_CFG_INFO3_STRU;

//connection authentication
typedef struct
{
    unsigned char username[12];
    unsigned char password[12];
} CCHEX_CONNECTION_AUTHENTICATION_STRU;

//download by employeeId and time
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    int record_num;
} CCHEX_RET_GET_RECORD_NUMBER_STRU;
/***********************************************
 * EmployeeId   用户号全部为0xFF 表示所有员工
 * start_date   开始时间为0xFF  表示不限制开始时间
 * end_date     结束时间为0xFF  表示不限制结束时间
 ***********************************************/
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //员工号
    unsigned char start_date[4];                //相距2000年后的秒数2000.1.2
    unsigned char end_date[4];                  //相距2000年后的秒数2000.1.2
}CCHEX_GET_RECORD_INFO_BY_TIME;                 //13B

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //员工号
    unsigned char start_date[4];                //相距2000年后的秒数2000.1.2
    unsigned char end_date[4];                  //相距2000年后的秒数2000.1.2
}CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID;                 //13B

typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //
    unsigned char date[4];                      //日期时间
    unsigned char back_id;                      //备份号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种        (ONLY use 3bytes )
    unsigned char padding[2];
    int CurIdx;
    int TotalCnt;
} CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //
    unsigned char date[4];                      //日期时间
    unsigned char back_id;                      //备份号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种        (ONLY use 3bytes )
    unsigned char padding[2];
    int CurIdx;
    int TotalCnt;
} CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID;

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //
    unsigned char date[4];                      //日期时间
    unsigned char back_id;                      //备份号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种        (ONLY use 3bytes )
}CCHEX_UPLOAD_RECORD_INFO_STRU;
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //
    unsigned char date[4];                      //日期时间
    unsigned char back_id;                      //备份号
    unsigned char record_type;                  //记录类型
    unsigned char work_type[3];                 //工种        (ONLY use 3bytes )
}CCHEX_UPLOAD_RECORD_INFO_STRU_VER_4_NEWID;

typedef struct
{
    unsigned char fail_alarm_time;              //0:do not alarm,  fail (1-10) to alarm
    unsigned char tamper_alarm;                 //0:close, 1:open
    unsigned char reserved[94];
} CCHEX_SET_BASIC_CFG_INFO5_STRU;               // 96 bytes
typedef struct
{
    unsigned int MachineId;
    int Result;                                 //0 ok, -1 err
    CCHEX_SET_BASIC_CFG_INFO5_STRU config;
} CCHEX_RET_GET_BASIC_CFG_INFO5_STRU;               // 104 bytes
typedef struct
{
    unsigned int    MachineId;
    int             Result;                                 //0 ok, -1 err
    unsigned int    card_id;
} CCHEX_RET_GET_CARD_ID_STRUF;                      //12 bytes

typedef struct
{
    unsigned char   alarm_stop;                   //==0xFF do nothing
    unsigned char   relay_set_status;                  //relay type 0:auto 1:open 2:close
    unsigned char   reserved[94];                 
} CCHEX_SET_DEV_CURRENT_STATUS_STRU;            // 96 bytes

typedef struct
{                              
    unsigned char   Dns[4];   
    char            Url[100];                               
} CCHEX_SET_URL_STRU;    
typedef struct
{
    unsigned int    MachineId;
    int             Result;                                 //0 ok, -1 err
    unsigned char   Dns[4];   
    char            Url[100];                               
} CCHEX_RET_GET_URL_STRU;                           // 112 bytes

#define CCHEX_STATUS_SWITCH_ONE_DAY  CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF

//Status Switch
typedef struct
{
    unsigned char   group_id;
    CCHEX_STATUS_SWITCH_ONE_DAY     day_week[7];
    unsigned char   status_id;
    char            padding[2];
}CCHEX_SET_STATUS_SWITCH_STRU;
typedef struct
{
    unsigned int    MachineId;
    int             Result; //0 ok, -1 err
    unsigned char   group_id;
    CCHEX_STATUS_SWITCH_ONE_DAY     day_week[7];
    unsigned char   status_id;
    char            padding[2];
}CCHEX_RET_GET_STATUS_SWITCH_STRU;

//Status Switch EXT
typedef struct
{
    unsigned char StartHour;
    unsigned char StartMin;
    unsigned char EndHour;
    unsigned char EndMin;
    unsigned char status_id;
}CCHEX_ONE_TIMER_STATUS; //5
typedef struct
{
    unsigned char   flag_week;                      //flag_week bit0-6:(0000000:6,5,4,3,2,1,sun     1:select)
    CCHEX_ONE_TIMER_STATUS     one_time[8];
    char            padding[3];
}CCHEX_SET_STATUS_SWITCH_STRU_EXT;
typedef struct
{
    unsigned int    MachineId;
    int             Result; //0 ok, -1 err
    unsigned char   flag_week;                      //flag_week bit0-6:(0000000:6,5,4,3,2,1,sun     1:select)
    CCHEX_ONE_TIMER_STATUS     one_time[8];
    char            padding[3];
}CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT;

typedef struct
{
    unsigned int    MachineId;
    int             Result;                 //0 ok, -1 err
    int             verify_status;          //0:ing     1:complete
    int             verify_ret;             //0:succeed 1:fail       
}CCHEX_RET_UPDATEFILE_STATUS;

typedef struct
{
    unsigned int MachineId;
    int Result;
    unsigned int cur_machineid;
} CCHEX_RET_GET_MACHINE_ID_STRU;
typedef struct
{
    unsigned int MachineId;
    int Result;
    unsigned int cur_machineid;
    unsigned int old_machineid;
} CCHEX_RET_SET_MACHINE_ID_STRU;


// get employee info
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID]; // 5 bytes to save
} CCHEX_GET_ONE_EMPLOYEE_INFO_STRU; // 5 bytes
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID]; // 28 bytes to save
} CCHEX_GET_ONE_EMPLOYEE_INFO_STRU_ID_VER_4_NEWID; // 28 bytes


typedef struct
{
    unsigned char start_date[4];                //相距2000年后的秒数2000.1.2
    unsigned char end_date[4];                  //相距2000年后的秒数2000.1.2
    unsigned char CmdType;                      /*扩展命令字：　0x00 获取当前时间段内总条数  0x01 下载记录 0x02  删除当前时间段内所有记录 0x03 设置实时上报日志记录0x04 获取实时上报记录状态 */
    unsigned char AutoFlag;                     //0:关闭实时上报日志记录 1:开启实时上报日志记录
}CCHEX_MANAGE_LOG_RECORD;                       //10B

typedef struct
{
    unsigned int MachineId;
    unsigned int CmdType;
    int Result;
    unsigned int IsAuto;
    unsigned int TotalNum;
    unsigned int CurNum;
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //
    unsigned char Date[4];                      //日期时间
    unsigned char LogType[2];                   //日志类型 0x0001 开门 0x0002 关门 0x0003 门磁警报 0x0004 防拆警报 0x0005 出门按钮 0x0006 破门
    unsigned char LogLen[2];                    //日志内容长度
    unsigned char padding[3];                   //填充对齐 无效数据
}CCHEX_RET_MANAGE_LOG_RECORD;                   //36B

/***********************************
 * 功能 : 获取照片文件总数,获取照片文件头信息,获取指定照片文件,删除指定照片文件
************************************/
typedef struct
{   
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    unsigned int PictureTotal;
} CCHEX_RET_GET_PICTURE_NUMBER_STRU;
typedef struct
{   
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    int CurIdx;
    int TotalCnt;
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];  //
    unsigned char DateTime[4];                 //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU;
typedef struct
{   
    unsigned int MachineId;
    int Result; //0 ok, -1 err
    int CurIdx;
    int TotalCnt;
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID;

typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];              //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_PICTURE_BY_EID_AND_TIME;
typedef struct
{
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID;
typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned int  DataLen;
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];              //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
    unsigned char Padding[3];  
    char          PictureData[];                            //图片内容
} CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME;
typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned int  DataLen;                                  //图片长度
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
    char          PictureData[];                            //图片内容
} CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID;
typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID];              //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME;
typedef struct
{
    unsigned int  MachineId;
    int Result; //0 ok, -1 err
    unsigned char EmployeeId[MAX_EMPLOYEE_ID_VER_4_NEWID];  //
    unsigned char DateTime[4];                              //日期时间为相距2000年后的秒数2000.1.2
} CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID;


/***********************************************************************
 *  功能 : 获取测温记录数量     RecordTpye:0-所有类型,10-测温正常,20-测温异常      现支持型号:FACE7T    FACE7TM
 **********************************************************************/
typedef struct
{
    unsigned int MachineId; //机器号
    int Result;             //0 ok, -1 err
    int RecordNum;          //记录数量
    int RecordTpye;         //测温类型
} CCHEX_RET_GET_T_RECORD_NUMBER_STRU;

/***********************************************************************
 * 获取测温记录         现支持型号:FACE7T    FACE7TM
 **********************************************************************/
typedef struct
{
    unsigned int MachineId;                    //机器号
    int Result;                                //0 ok, -1 err
    unsigned char RecoradId[8];                //测温记录ID
    unsigned char Date[4];                     //日期时间2000.01.02后的秒数
    unsigned char Temperature[2];              //温度整数/10  368 :36.8
    unsigned char TemperatureType;              //10-测温正常,20-测温异常 
    unsigned char MaskType;                     //是否戴口罩 0没戴，1戴了，2没检测
    
    unsigned int CurIdx;
    unsigned int TotalCnt;
} CCHEX_RET_GET_T_RECORD_STRU;
/***********************************************************************
 * 获取测温记录ID的图片         现支持型号:FACE7T    FACE7TM
 **********************************************************************/
typedef struct
{
    unsigned int  MachineId;                        //机器号
    int Result; //0 ok, -1 err
    unsigned char RecoradId[8];                     //测温记录ID
    unsigned char TemperatureType;                  //10-测温正常,20-测温异常 
    unsigned char Padding[3];                       //对齐(无效)
    unsigned int  DataLen;                          //图片长度
    char          PictureData[];                    //图片内容
} CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU;
/***********************************************************************
 * 删除测温记录ID的图片         现支持型号:FACE7T    FACE7TM
 **********************************************************************/
typedef struct
{
    unsigned char RecoradId[8];                     //测温记录ID
    unsigned char TemperatureType;                  //0-所有类型 10-测温正常,20-测温异常 
} CCHEX_PICTURE_BY_RECORD_ID_STRU;

typedef struct
{
    unsigned int MachineId;                         //机器号
    int Result;                                     //0 ok, -1 err
    unsigned char RecoradId[8];                     //测温记录ID
    unsigned char TemperatureType;                  //0-所有类型 10-测温正常,20-测温异常 
} CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU;

/*****************************************************************************
 *
 * 
 * 
 * 
 *  SAC822   门禁控制器     开始
 * 
 * 
 * 
 * 
 *****************************************************************************/
#define MAX_GET_RS_COUNT_500	500
#define MAX_GET_RS_COUNT_50		50
#define MAX_GET_RS_COUNT_1		1
#define MAX_GET_RS_COUNT_2		2
/*****************************************************************************
 * SAC822   类型和对应解读结构体:
 *****************************************************************************/
enum CMD_PRINCIPAL{
    CMD_Pri_Unknow              =  0,  //
    CMD_Pri_UdpDevice           =  1,  //Udp设备：搜索，设置        (下载(搜索)/上传(设置))                          
    CMD_Pri_AtIniConfig         =  2,  //设备配置：下载，上传        (下载/上传(设置))                             
    CMD_Pri_AtRecord            =  3,  //记录                       (下载/上传)                  下载上传最大数: 500                     
    CMD_Pri_MaType              =  4,  //设备类型                    (下载/上传)                      1    
    CMD_Pri_MaDateTime          =  5,  //设备时间                    (下载/上传)                      1       
    CMD_Pri_NetPara             =  6,  //网络参数                   (下载/上传)                       1     
    CMD_Pri_DeviceURL           =  7,  //设备URL                    (下载/上传)                       1         
    CMD_Pri_DoorStatus          =  8,  //门状态                    (下载/上传)                        2 
    CMD_Pri_AtEvent             =  9,  //设备事件                   (下载/推送)                    下载上传最大数: 500
    CMD_Pri_AtDelaySet          = 10,  //延时设置                    (下载/上传(设置))                 2
    CMD_Pri_AtEventType         = 11,  //设备事件定义                 (下载)                           500

    CMD_Pri_StaffInfo           = 50,  //员工信息                   (下载/上传/删除/初始化)         下载上传最大数: 500
    CMD_Pri_StaffGroup          = 51,  //员工组                     (下载/上传/删除)               下载上传最大数: 50
    CMD_Pri_StaffCombi          = 52,  //员工组合(员工+员工组)       (下载/上传/删除)               下载上传最大数: 500
    CMD_Pri_Door                = 54,  //门                         (下载/上传/删除)               下载上传最大数: 50
    CMD_Pri_DoorGroup           = 55,  //门组                       (下载/上传/删除)               下载上传最大数: 50
    CMD_Pri_DoorCombi           = 56,  //门组合(门+门组)             (下载/上传/删除)               下载上传最大数: 500
    CMD_Pri_TimeSpace           = 57,  //门禁时段                    (下载/上传/删除)               下载上传最大数: 50
    CMD_Pri_TimeSpaceGroup      = 58,  //门禁时段组                  (下载/上传/删除)               下载上传最大数: 50
    CMD_Pri_TimeSpaceCombi      = 59,  //门禁时段组合(时段+时段组)    (下载/上传/删除)               下载上传最大数: 500
    CMD_Pri_DoorTimeSpaceCombi  = 60,  //门禁时段组合----门          (下载/上传/删除)               下载上传最大数: 50 
    CMD_Pri_FirstCardGroup      = 61,  //首卡组                     (下载/上传/删除)               下载上传最大数:  50
    CMD_Pri_FirstCardCombi      = 62,  //首卡组合(员工组+首卡组)      (下载/上传/删除)              下载上传最大数:  500
    CMD_Pri_DoorFirstCardCombi  = 63,  //首卡组合----门              (下载/上传/删除)              下载上传最大数:  50
    CMD_Pri_MultiCGroup         = 64,  //多卡组                     (下载/上传/删除)               下载上传最大数:  50
    CMD_Pri_MultiCGroupIDCount  = 65,  //多卡组拼合(5个多卡组及其内员工数)    (下载/上传/删除)       下载上传最大数:   500
    CMD_Pri_MultiCCombi         = 66,  //多卡组组合(员工+多卡组)             (下载/上传/删除)       下载上传最大数:   500
    CMD_Pri_DoorMultiCCombi     = 67,  //多卡组拼合----门            (下载/上传/删除)               下载上传最大数:   50
    CMD_Pri_Holidays            = 68,  //节假日设置　　　            (下载/上传/删除)               下载上传最大数:    50
    CMD_Pri_AntiBack            = 69,  //反潜信息记录表              (下载/上传/删除)               下载上传最大数:    500
    CMD_Pri_TaskTimed           = 70,  //定时任务表                  (下载/上传/删除)                   500
    CMD_Pri_TaskTimedLog        = 71,  //定时任务日志                (下载/删除)                        500
    CMD_Pri_LinkageSetting      = 72,  //联动设置                    (下载/上传/删除)                   500
};
enum CMD_ACTION{                //命令动作
    CMD_Act_Unknow = 0,         //
    CMD_Act_Down   = 1,         //下载，搜索
    CMD_Act_Upload = 2,         //上传，设置
    CMD_Act_Delete = 3,         //删除
    CMD_Act_Empty  = 4,         //清空，初始化
};
enum CMD_ACTION_RES{                 //命令动作应答
    CMD_Act_RES_Unknow =  8,         //
    CMD_Act_RES_Down   =  9,         //下载，搜索
    CMD_Act_RES_Upload = 10,         //上传，设置
    CMD_Act_RES_Delete = 11,         //删除
    CMD_Act_RES_Empty  = 12,         //清空，初始化
    CMD_Act_RES_Push   = 13,         //推送 
};
enum _type_delete{
//        Type_Del_FP1        =   1,
//        Type_Del_FP         =   2,
        Type_Del_FP_ALL     =   3,  //删除全部指纹
        Type_Del_PWD        =   4,  //删除密码
        Type_Del_Card       =   8,  //删除卡号
        Type_Del_ALL        = 255,  //删除全部
    };
/**********************************
 * 下载通用返回结构                 Data[]: SAC 对应类型
 **********************************/
typedef struct
{
    unsigned int  MachineId;        //机器号
    unsigned int CmdPrincipa;       //执行的主体操作见类型:CMD_PRINCIPAL
    int Result;                     //==0 :成功 ;  !=0 : 失败
    unsigned int SeqNum;            //获取的的信息体当前下载次数(从0开始)
    unsigned int MaxCount;          //每次获取的的信息体最大数量
    unsigned int DataCurCount;      //获取的的信息体当次数量,DataCurCount小于MaxCount为最后一次
    // char Data[];                 //数据
}SAC_DOWNLOAD_COMMON_RESULT;        
/**********************************
 * 上传通用返回结构                Data[]: SAC 对应类型
 **********************************/
typedef struct
{
    unsigned int  MachineId;        //机器号
    unsigned int CmdPrincipa;       //执行的主体操作见类型:CMD_PRINCIPAL
    int Result;                     //==0 : 全部成功(不返回数据); !=0 : 表示,部分失败或者全部失败
    unsigned int DataCurCount;      //执行的信息体数量
    // char Data[];                 //每个byte表示一个信息体执行结果
}SAC_UPLOAD_COMMON_RESULT; 
/**********************************
 * 删除通用返回结构                 Data[]: SAC 对应类型
 **********************************/
typedef struct
{
    unsigned int  MachineId;        //机器号
    unsigned int CmdPrincipa;       //执行的主体操作见类型:CMD_PRINCIPAL
    int Result;                     //==0 : 全部成功(不返回数据); !=0 : 表示,部分失败或者全部失败
    unsigned int DataCurCount;      //执行的信息体数量
    // char Data[];                 //每个byte表示一个信息体执行结果
}SAC_DELETE_COMMON_RESULT;
/**********************************
 * 初始化通用返回结构
 **********************************/
typedef struct
{
    unsigned int  MachineId;        //机器号
    unsigned int CmdPrincipa;       //执行的主体操作见类型:CMD_PRINCIPAL
    int Result;                     //==0 :成功;   !=0 : 失败
}SAC_INIT_COMMON_RESULT;
/**********************************
 * 推送通用返回结构
 **********************************/
typedef struct
{
    unsigned int  MachineId;        //机器号
    unsigned int CmdPrincipa;       //执行的主体操作见类型:CMD_PRINCIPAL
    int Result;                     //==0 :成功 ;  !=0 : 失败
    unsigned int DataCurCount;      //推送的的信息体当次数量
    // char Data[];                 //数据
}SAC_PUSH_COMMON_RESULT;        



/**********************************
 * SAC 对应类型
 **********************************/
/*                          对应类型:    CMD_Pri_StaffInfo  
SAC_DATA_RS_EmInfoEx:           上传下载    500
    uEmID           :员工号
    wPwd            :密码位数 = Password[0] >> 4
                    (Password[0]&0xff)<<16 + (Password[1]<<8) + Password[2]   左边补零到 密码位数
                    若 Password[3] 返回全为0xFF表明密码不存在
    mCarID            :   (Card[0]<<24)+ (Card[1]<<16)+ (Card[2]<<8)+ Card[3] 
                    若 Card[4] 返回全为0xFF表明卡号不存在 
    szName          :为UNICODE 人名
    uCardType       :卡类型 0 普通卡，1 首卡常开卡, 2 多卡开门卡
    uGroupID        :组号
    uCheckMode      :考勤模式
    wFPRecordInfo        :指纹登记情况定义：位0 = 1表示已登记指纹1，位1 = 1表示已登记指纹2 位0-9 表示 1-10个指纹
    PwdH8bit        :高位密码,暂未使用
    HolidayGroup    :节假日组
                    节假日组号：
                    卡类型：0 普通卡，1 首卡常开卡, 2 多卡开门卡
    uCharacter         :特殊信息 位7-6：权限 1-普通用户	3-管理员   0x01000000  0x11000000 
SAC_DATA_RS_EmInfoEx_Delete:    下砸 
    uEmID           :员工号
    uDeleteType     :删除方式 类型见_type_delete
*/
typedef struct
{
    unsigned char uEmID[MAX_EMPLOYEE_ID];      // 
    unsigned char wPwd[3];                      // 
    unsigned char mCarID[4]; 
    unsigned char szName[20]; 
    unsigned char uCardType; 
    unsigned char uGroupID; 
    unsigned char uCheckMode; 
    unsigned char wFPRecordInfo[FP_STATUS_LEN];
    unsigned char PwdH8bit;
    unsigned char HolidayGroup;
    unsigned char uCharacter;                      
}SAC_DATA_RS_EmInfoEx;                      //上传
typedef struct
{
    unsigned char uEmID[MAX_EMPLOYEE_ID]; 
    unsigned char uDeleteType;          
}SAC_DATA_RS_EmInfoEx_Delete;

/*                          对应类型:    CMD_Pri_StaffGroup  
SAC_DATA_RS_EmGroup:            上传下载    50
    组信息:
    uGroupID     :组编号
    szName   :组名
    uGroupType  :组类型
SAC_DATA_RS_EmGroup_Delete:     删除
    uGroupID
*/
typedef struct
{
    unsigned char uGroupID;
    unsigned char szName[100];
    unsigned char uGroupType;
}SAC_DATA_RS_EmGroup;
typedef struct
{
    unsigned char uGroupID;
}SAC_DATA_RS_EmGroup_Delete;


/*                          对应类型:    CMD_Pri_StaffCombi  
SAC_DATA_RS_EmGroupCombine:            上传下载    500
    员工的组合:
    uEmID  :用户号
    uGroupID     :组编号
SAC_DATA_RS_EmGroupCombine_Delete:      删除
*/
typedef struct
{
    unsigned char uEmID[MAX_EMPLOYEE_ID];//5
    unsigned char uGroupID;
}SAC_DATA_RS_EmGroupCombine;

#define SAC_DATA_RS_EmGroupCombine_Delete SAC_DATA_RS_EmGroupCombine

/*                          对应类型:    CMD_Pri_Door  
SAC_DATA_RS_Door:            上传下载    50
    门信息:
    uDoorID             :门号
    szName              :门名
    szDevName           :设备名(门名称默认值: “设备名-门编号”   (UNICODE编码))
    uAntiBackMode       :反潜类型 0：未开启反潜 1：单门反潜 2：双门反潜
    uInterLockFlag      :是否互锁   0: 未开启     1：开启
    InterLockDoorID     :互锁门号   (互锁门号不能与当前门相同门号)
    state               :门状态   0：重置  1：普通  2：多卡开门   3：首卡常开 4：常关 5：常开 6：错误
    doorPwd             :密码
SAC_DATA_RS_Door_Delete:     删除
    uDoorID             :门号
*/
typedef struct 
{
    unsigned char uDoorID;
    unsigned char szName[100];
    unsigned char szDevName[40];
    unsigned char uAntiBackMode;
    unsigned char uInterLockFlag;
    unsigned char InterLockDoorID;
    unsigned char state;
    unsigned char doorPwd[3];
}SAC_DATA_RS_Door;
typedef struct 
{
    unsigned char uDoorID;
}SAC_DATA_RS_Door_Delete;

/*                          对应类型:    CMD_Pri_DoorGroup  
SAC_DATA_RS_DoorGroup:            上传下载    50
    门组信息:
    uGroupID        :组号
    szName          :组名
SAC_DATA_RS_DoorGroup_Delete:      删除
    uGroupID        :组号
*/
typedef struct
{
    unsigned char uGroupID;
    unsigned char szName[100];
}SAC_DATA_RS_DoorGroup;
typedef struct 
{
    unsigned char uGroupID;
}SAC_DATA_RS_DoorGroup_Delete;
/*                          对应类型:    CMD_Pri_DoorCombi  
SAC_DATA_RS_DoorGroupAndDoor:            上传下载    500
    门组合信息:门 与 门组 关联信息
    uID             :组合号
    uGroupID        :门号
    uDoorID         :门组号
SAC_DATA_RS_DoorGroupAndDoor_Delete:    删除
*/
typedef struct
{
    unsigned char uID;
    unsigned char uGroupID;
    unsigned char uDoorID;
}SAC_DATA_RS_DoorGroupAndDoor;
typedef struct
{
    unsigned char uID;
}SAC_DATA_RS_DoorGroupAndDoor_Delete;


/*                          对应类型:    CMD_Pri_TimeSpace  
SAC_DATA_RS_Time:               上传下载    50
    上传 下载时段信息:
    uSpaceID:时间段序号
    date        :mWeek1Mon: 星期一子时段,mWeek2Tue: 星期二子时段,mWeek3Wed: 星期三子时段,mWeek4Tur: 星期四子时段,mWeek5Fri: 星期五子时段,
                 mWeek6Sat: 星期六子时段,mWeek7Sun: 星期日子时段,mHoliday1: 节假日一时段,mHoliday2: 节假日二时段,mHoliday3: 节假日三时段
            unsigned char m_StartHour :开始小时
            unsigned char m_StartMinu  :开始分钟
            unsigned char m_EndHour   :开始小时
            unsigned char m_EndMinu    :开始分钟
SAC_DATA_RS_Time_Delete:        删除
*/
typedef struct
{
    unsigned char m_StartHour;
    unsigned char m_StartMinu;
    unsigned char m_EndHour;
    unsigned char m_EndMinu;
}SAC_DATA_RS_Time_One;
typedef struct 
{
    unsigned char uSpaceID;
    SAC_DATA_RS_Time_One mWeek1Mon;
    SAC_DATA_RS_Time_One mWeek2Tue;
    SAC_DATA_RS_Time_One mWeek3Wed;
    SAC_DATA_RS_Time_One mWeek4Tur;
    SAC_DATA_RS_Time_One mWeek5Fri;
    SAC_DATA_RS_Time_One mWeek6Sat;
    SAC_DATA_RS_Time_One mWeek7Sun;
    SAC_DATA_RS_Time_One mHoliday1;
    SAC_DATA_RS_Time_One mHoliday2;
    SAC_DATA_RS_Time_One mHoliday3;
}SAC_DATA_RS_Time;
typedef struct 
{
    unsigned char uSpaceID;
}SAC_DATA_RS_Time_Delete;

/*                          对应类型:    CMD_Pri_TimeSpaceGroup  
SAC_DATA_RS_TimeGroup:            上传下载    50
    时间段组信息:
    uGroupID        :组号
    szName          :组名
SAC_DATA_RS_TimeGroup_Delete:      删除
    uGroupID        :组号
*/
typedef struct
{
    unsigned char uGroupID;
    unsigned char szName[100];
}SAC_DATA_RS_TimeGroup;
typedef struct 
{
    unsigned char uGroupID;
}SAC_DATA_RS_TimeGroup_Delete;



/*                          对应类型:    CMD_Pri_TimeSpaceCombi  
SAC_DATA_RS_TimeGroupAndTime:            上传下载    500
    时间段组合信息:时间段 与 时间段组 关联信息
    uID         :组合号
    uTimeID     :时段号
    uGroupID    :时段组号
SAC_DATA_RS_TimeGroupAndTime_Delete:    删除
*/
typedef struct
{
    unsigned char uID;
    unsigned char uGroupID;
    unsigned char uTimeID;
}SAC_DATA_RS_TimeGroupAndTime;
typedef struct
{
    unsigned char uID;
}SAC_DATA_RS_TimeGroupAndTime_Delete;


/*                          对应类型:    CMD_Pri_DoorTimeSpaceCombi  
SAC_DATA_RS_OpenDoor_General:            上传下载    50
    门禁时段组信息:
    uGroupID            :组编号
    szName              :组名
    uUserGroupID        :用户组号
    uDoorGroupID        :门组号
    uTimeSpaceGroupID   :时段组号
SAC_DATA_RS_OpenDoor_General_Delete:    删除
    uGroupID            :组编号
*/
typedef struct 
{
    unsigned char uGroupID;
    unsigned char szName[100];
    unsigned char uUserGroupID;
    unsigned char uDoorGroupID;
    unsigned char uTimeSpaceGroupID;
}SAC_DATA_RS_OpenDoor_General;           
typedef struct 
{
    unsigned char uGroupID;
}SAC_DATA_RS_OpenDoor_General_Delete;  
/*                          对应类型:    CMD_Pri_FirstCardGroup  
SAC_DATA_RS_FirstCardGroup:            上传下载    50
    首卡常开组数据体:
    uGroupID            :组编号
    szName              :组名
SAC_DATA_RS_FirstCardGroup_Delete:    删除
    uGroupID            :组编号
*/
typedef struct 
{
    unsigned char uGroupID;
    unsigned char szName[100];
}SAC_DATA_RS_FirstCardGroup;
typedef struct
{
    unsigned char uGroupID;
}SAC_DATA_RS_FirstCardGroup_Delete;
/*                          对应类型:    CMD_Pri_FirstCardCombi  
SAC_DATA_RS_FirstCardGroupAndUser:            上传下载    500
    1首卡组合数据体:
    uID         :组合号
    uEmID       :用户号
    uGroupID    :组编号
SAC_DATA_RS_FirstCardGroupAndUser_Delete:    删除
    uID         :组合号
*/
typedef struct 
{
    unsigned char uID;
    unsigned char uEmID[MAX_EMPLOYEE_ID];//5
    unsigned char uGroupID;
}SAC_DATA_RS_FirstCardGroupAndUser;
typedef struct
{
    unsigned char uID;
}SAC_DATA_RS_FirstCardGroupAndUser_Delete;

/*                          对应类型:    CMD_Pri_DoorFirstCardCombi  
SAC_DATA_RS_OpenDoor_FirstCard:            上传下载    50
    首卡门禁组:
    uGroupID                :组编号
    szName                  :组名
    uFirstCardUserGroupID   :首卡组合号
    uDoorGroupID            :门组号
    uTimeSpaceGroupID       :时段组号
SAC_DATA_RS_OpenDoor_FirstCard_Delete:    删除
    uGroupID                :组编号
*/
typedef struct 
{
    unsigned char uGroupID;
    unsigned char szName[100];
    unsigned char uFirstCardUserGroupID;
    unsigned char uDoorGroupID;
    unsigned char uTimeSpaceGroupID;
}SAC_DATA_RS_OpenDoor_FirstCard;
typedef struct
{
    unsigned char uGroupID;
}SAC_DATA_RS_OpenDoor_FirstCard_Delete;

/*                          对应类型:    CMD_Pri_MultiCGroup  
SAC_DATA_RS_MultipleCardGroup:            上传下载    50
    多卡开门组:
    uGroupID            :组编号
    szName              :组名
SAC_DATA_RS_MultipleCardGroup_Delete:    删除
    uGroupID            :组编号
*/
typedef struct 
{
    unsigned char uGroupID;
    unsigned char szName[100];
}SAC_DATA_RS_MultipleCardGroup;
typedef struct
{
    unsigned char uGroupID;
}SAC_DATA_RS_MultipleCardGroup_Delete;

/*                          对应类型:    CMD_Pri_MultiCGroupIDCount  
SAC_DATA_RS_MultipleCardGroupCombine:            上传下载    500
    多卡开门组号及个数:
    uID            :组编号
    .......
CSAC_DATA_RS_MultipleCardGroupCombine_Delete:    删除
    uID            :组编号
*/
typedef struct
{
    unsigned char uID;
    unsigned char uFirstGroupID;
    unsigned char uFirstCount;
    unsigned char uSecordGroupID;
    unsigned char uSecordCount;
    unsigned char uuThirdGroupID;
    unsigned char uuThirdCount;
    unsigned char uuFourthGroupID;
    unsigned char uuFourthCount;
    unsigned char uuFifthGroupID;
    unsigned char uuFifthCount;
}SAC_DATA_RS_MultipleCardGroupCombine;
typedef struct
{
    unsigned char uID;
}CSAC_DATA_RS_MultipleCardGroupCombine_Delete;

/*                          对应类型:    CMD_Pri_MultiCCombi  
SAC_DATA_RS_MultipleCardGroupAndUser:            上传下载    500
    多卡开门组合:
    uID         :组合号
    uEmID       :用户号
    uGroupID    :组编号
SAC_DATA_RS_MultipleCardGroupAndUser_Delete:    删除
    uID         :组合号
*/
typedef struct
{
    unsigned char uID;
    unsigned char uEmID[MAX_EMPLOYEE_ID];//5
    unsigned char uGroupID;
}SAC_DATA_RS_MultipleCardGroupAndUser;
typedef struct
{
    unsigned char uID;
}SAC_DATA_RS_MultipleCardGroupAndUser_Delete;

/*                          对应类型:    CMD_Pri_DoorMultiCCombi  
SAC_DATA_RS_OpenDoor_MultipleCard:            上传下载    50
    多卡开门门禁组:
    uGroupID                    :组合号
    szName                      :组名
    uMultipleGroupCombineID     :多卡开门组号
    uDoorGroupID                :门组号
    uTimeSpaceGroupID           :时间段组号
SAC_DATA_RS_OpenDoor_MultipleCard_Delete:    删除
    uGroupID         :组合号
*/
typedef struct
{
    unsigned char uGroupID;
    unsigned char szName[100];
    unsigned char uMultipleGroupCombineID;
    unsigned char uDoorGroupID;
    unsigned char uTimeSpaceGroupID;
}SAC_DATA_RS_OpenDoor_MultipleCard;
typedef struct
{
    unsigned char uGroupID;
}SAC_DATA_RS_OpenDoor_MultipleCard_Delete;

/*                          对应类型:    CMD_Pri_Holidays  
SAC_RS_Holiday:            上传下载    50
    节假日:
    uHolidayID                  :节假日号
    szName                      :节假日名
    uType                       :类型
    uStartYear                  :开始年
    uStartMonth                 :开始月
    uStartDay                   :开始日
    uEndYear                    :结束年
    uEndMonth                   :结束月
    uEndDay                     :结束日
    uYearFlag                   :年标记
SAC_RS_Holiday_Delete:    删除
    uGroupID         :组合号
*/
typedef struct 
{
    unsigned char uHolidayID;//1
    unsigned char szName[100];//100
    unsigned char uType;//1
    unsigned char uStartYear[2];//2
    unsigned char uStartMonth;//1
    unsigned char uStartDay;//1
    unsigned char uEndYear[2];//2
    unsigned char uEndMonth;//1
    unsigned char uEndDay;//1
    unsigned char uYearFlag;//1
}SAC_RS_Holiday;
typedef struct 
{
    unsigned char uHolidayID;//1
}SAC_RS_Holiday_Delete;
/*                          对应类型:    CMD_Pri_AntiBack  
SAC_DATA_RS_AntiBack:            上传下载    500
    日志信息表:
    uEmID                       :用户号
    dwTimeIn                    :入门时间
    dwTimeOut                   :出门时间
    dwDoorIn                    :入门
    dwDoorOut                   :出门
    uFlag                       :类型
    uSpaceID                    :
    uHolidays                   :节假日

SAC_DATA_RS_AntiBack_Delete:    删除
    uEmID                   :用户号
    dwDoorIn                :入门
*/
typedef struct
{
    unsigned char uEmID[MAX_EMPLOYEE_ID];   //5
    unsigned char dwTimeIn[4];              //4
    unsigned char dwTimeOut[4];             //4
    unsigned char dwDoorIn[2];              //2
    unsigned char dwDoorOut[2];             //2
    unsigned char uFlag;                    //1
    unsigned char uSpaceID;                 //1
    unsigned char uHolidays;                //1
}SAC_DATA_RS_AntiBack;
typedef struct
{
    unsigned char uEmID[MAX_EMPLOYEE_ID];   //5
    unsigned char dwDoorIn[2];              //2
}SAC_DATA_RS_AntiBack_Delete;

/*                          对应类型:    CMD_Pri_AtRecord  
SAC_DATA_RS_Record:            上传下载    500
    日志信息表:
    uRowID                      :日志号
    uEmID                       :用户号
    dwTime                      :时间
    uDeviceID                   :设备号
    uSpaceID                    :
    uCType                      :类型
    uGroupID                    :组号
    uHolidayID                  :节假日号
    uPassType                   :

SAC_DATA_RS_Record_Delete:    删除
    uRowID                  :日志号
*/
typedef struct{
    unsigned char uChar01;
    unsigned char uChar02;
    unsigned char uChar03;
    unsigned char uChar04;
    unsigned char uChar05;
    unsigned char uChar06;
    unsigned char uChar07;
    unsigned char uChar08;
}longWORD;
typedef struct
{
    longWORD uRowID;	//8//上传时无意义
    unsigned char uEmID[MAX_EMPLOYEE_ID];//5
    unsigned char dwTime[4];//4
    unsigned char uDeviceID;//1
    unsigned char uSpaceID;//1
    unsigned char uCType;//1
    unsigned char uGroupID;//1
    unsigned char uHolidayID;//1
    unsigned char uPassType;//1
}SAC_DATA_RS_Record;
typedef struct
{
    longWORD uRowID;//8
}SAC_DATA_RS_Record_Delete;

/*                          对应类型:    CMD_Pri_MaType  
SAC_DATA_Device_MaType:            设置获取    1
    设备类型:
    uty1    :
    uty2    :
    ......
*/
typedef struct
{
    unsigned char uty1;
    unsigned char uty2;
    unsigned char uty3;
    unsigned char uty4;
    unsigned char uty5;
    unsigned char uty6;
    unsigned char uty7;
    unsigned char uty8;
}SAC_DATA_Device_MaType;
/*                          对应类型:    CMD_Pri_MaDateTime  
SAC_DATA_Device_Datetime:            设置获取    1
    设备时间   :
    szYear      :年
    szMonth     :月
    szDay       :日
    szHour      :时
    szMinute    :分
    szSecond    :秒
*/
typedef struct
{
    unsigned char szYear;
    unsigned char szMonth;
    unsigned char szDay;
    unsigned char szHour;
    unsigned char szMinute;
    unsigned char szSecond;
}SAC_DATA_Device_Datetime;
/*                          对应类型:    CMD_Pri_NetPara  
SAC_DATA_Device_Datetime:            设置获取    1
    设备网络设置   :
    szIpAddress :
    szMask      :
    szMac       :
    szGateWay   :
    szServer    :
    szMay       :
    wPort       :
    szMode      :
    DHCP        :
*/
typedef struct
{
    unsigned char uIPHH;
    unsigned char uIPHL;
    unsigned char uIPLH;
    unsigned char uIPLL;
}IPDWORD;
typedef struct
{
    unsigned char uMacHH;
    unsigned char uMacHL;
    unsigned char uMacMH;
    unsigned char uMacML;
    unsigned char uMacLH;
    unsigned char uMacLL;
}MACIP;
typedef struct SAC_DATA_Device_Net
{
    IPDWORD szIpAddress;
    IPDWORD szMask;
    MACIP szMac;
    IPDWORD szGateWay;
    IPDWORD szServer;
    unsigned char szMay;
    unsigned char wPort[2];
    unsigned char szMode;
    unsigned char DHCP;
}SAC_DATA_Device_Net;

/*                          对应类型:    CMD_Pri_DeviceURL  
SAC_DATA_Device_Url:            设置获取    1
    设备URL   :
*/
typedef struct
{
    IPDWORD dns;
    unsigned char uURL[100];//100
}SAC_DATA_Device_Url;

/*                          对应类型:    CMD_Pri_AtIniConfig  
SAC_DATA_Device_INI_Get:            设置获取    1
    配置信息   :   
    szRealEvent :推送信息
*/
enum ACTION_FirstCard{                //首卡３秒内刷Ｎ次动作
    ACTION_FirstCard_ToOpen   =  1,         //动作目标为常闭
    ACTION_FirstCard_ToClose  =  2,         //动作目标为常开
    ACTION_FirstCard_ToNormal =  3,         //动作目标为到设定状态
};
enum ACTION_FirstCard_Combine{                
//首卡３秒内刷Ｎ次动作组合，刷一次动作＆刷二次动作＆刷三次动作

    ACTION_FirstCard_Combine_OCN = 123,         
    //刷一次动作＆刷二次动作＆刷三次动作：常开＆常闭＆设定
    
    ACTION_FirstCard_Combine_ONC = 132,         
    //刷一次动作＆刷二次动作＆刷三次动作：常开＆设定＆常闭
    
    ACTION_FirstCard_Combine_CON = 213,         
    //刷一次动作＆刷二次动作＆刷三次动作：常闭＆常开＆设定
    
    ACTION_FirstCard_Combine_CNO = 231,         
    //刷一次动作＆刷二次动作＆刷三次动作：常闭＆设定＆常开
    
    ACTION_FirstCard_Combine_NOC = 312,         
    //刷一次动作＆刷二次动作＆刷三次动作：设定＆常开＆常闭
    
    ACTION_FirstCard_Combine_NCO = 321,         
    //刷一次动作＆刷二次动作＆刷三次动作：设定＆常闭＆常开
};
typedef struct
{
    char szVersion[8];//设置时此值无效
    unsigned char szPwd[3];
    unsigned char szDelay;
    unsigned char szVoice;
    unsigned char szLanguage;

    unsigned char szDateType;
    unsigned char szAtState;//设置时此值无效

    unsigned char szLevel;
    unsigned char szWiegandHead;
    unsigned char szWiegandOpt;

    unsigned char szWorkCode;
    unsigned char szSend;
    unsigned char szUpdate;
    unsigned char szRing;
    unsigned char szRorLDelay;

    unsigned char szWarning[3];
    unsigned char szAt;   
    unsigned char szAtMethod;

    unsigned char szFirstCardAction[2];
    //三秒内刷一次卡的动作/三秒内刷二次卡的动作/三秒内刷三次卡的动作
    unsigned char  szRealEvent;
    char szReverve[2];
}SAC_DATA_Device_INI_Get;

/*                          对应类型:    CMD_Pri_DoorStatus  
SAC_DATA_Device_DoorStatus:            设置获取    1
    门状态查询与设置   :
        uDoorNum        :门数
        uDoorAlarm      :门告警
        uDoorState      :门状态
*/
typedef struct
{
    unsigned char uDoorNum;
    unsigned char uDoorAlarm;
    unsigned char uDoorState;
}SAC_DATA_Device_DoorStatus_One;
typedef struct
{
    SAC_DATA_Device_DoorStatus_One oDoor1;
    SAC_DATA_Device_DoorStatus_One oDoor2;
}SAC_DATA_Device_DoorStatus;


/*                          对应类型:    CMD_Act_RES_Push  
SAC_DATA_Device_Event:            设置获取    1
    门状态查询与设置   :
        uEventType      :事件类型
        dwTime          :时间
        uEmID           :员工号
        uWiegandNo      :韦根号
        uDoorNo         :门号
        szMsg           :信息
*/
typedef struct
{
    unsigned char  uEventType;//1
    unsigned char  dwTime[4];//4
    unsigned char  uEmID[MAX_EMPLOYEE_ID];//5
    unsigned char  uWiegandNo;//1
    unsigned char  uDoorNo;//1
    unsigned char  szMsg[40];
}SAC_DATA_Device_Event;
/*****************************************************************************
 *
 * 
 * 
 * 
 *  SAC822   门禁控制器     结束
 * 
 * 
 * 
 * 
 *****************************************************************************/






/*****************************************************************************
 * [NUM : 2.2]
 *创建句柄前 初始化                                         
*****************************************************************************/
API_EXTERN void CChex_Init();


/*****************************************************************************
 * [NUM : 2.3]
 *   return AvzHandle                                        
 * 初始化后 创建句柄   open  port:5010
*****************************************************************************/
API_EXTERN void *CChex_Start();


/*****************************************************************************
 *   return AvzHandle
 * 初始化后 创建句柄 IsCloseServer:   0 : not close  ,ServerPort 0:random   other:other
 * Remove  fun:
 * API_EXTERN int CChex_Set_Service_Port(unsigned short Port); 
 * API_EXTERN int CChex_Set_Service_Disenable(); 
 * ADD:
 *  API_EXTERN void CChex_Start_With_Param(IsCloseServer,ServerPort)  replace CChex_Set_Service_Port(),CChex_Set_Service_Disenable(),CChex_Start() to set server and port Handle
 **********************************************************/
API_EXTERN void *CChex_Start_With_Param(unsigned int IsCloseServer,unsigned int ServerPort);


/*****************************************************************************
 * [NUM : 2.4]
 * 释放句柄      
*****************************************************************************/
API_EXTERN void CChex_Stop(void *CchexHandle);


/*****************************************************************************
 * 获取当前服务端端口号
 * 函数返回:端口号  
*****************************************************************************/
API_EXTERN int CChex_Get_Service_Port(void *CchexHandle);       //get service port:after CChex_Start()


/*****************************************************************************
 * [NUM : 2.1]
 * 获取SDK的版本号
 * 函数返回:版本号 
*****************************************************************************/
API_EXTERN unsigned int CChex_Version();


/*****************************************************************************
 * 设备连接时先进行此连接认证
*****************************************************************************/
API_EXTERN int CChex_Set_Connect_Authentication(void *CchexHandle,CCHEX_CONNECTION_AUTHENTICATION_STRU *Param);       //Set CONNECTION  AUTHENTICATION For Default


/*****************************************************************************
 * Return   -1 : disconnect ; >=0: connect  (DevIdx)  
 * Just For Zhao
 * 函数返回:  DevIdx : 设备SDK通信的连接号
******************************************************************************/
API_EXTERN int CChex_Find_DevIdx_By_MachineId(void *CchexHandle,unsigned int MachineId);       


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
API_EXTERN void CChex_SetSdkConfig(void *CchexHandle,int SetAutoDownload,int SetRecordflag,int SetLogFile);


/*****************************************************************************
 * [NUM : 2.5]
 * important fun
    return 
        >0  ok, return length  info  读取返回信息成功
        =0, no result.               没有返回信息
        <0  return (0 - need len)    读取返回信息时Buff长度不够
    主要函数,所有返回信息都是通过此函数读取出来,一处读取分派处理
    CchexHandle :句柄
    DevIdx      :接收 设备SDK通信的连接号
    Type        :接收 单次信息类型
    Buff        :接收 单次信息的buff
    Len         :传入Buff的大小
 *  读取返回信息后根据  enum
    {
        CCHEX_RET_RECORD_INFO_TYPE                  = 1,
        CCHEX_RET_DEV_LOGIN_TYPE                    = 2,
        ..............,
        ..................
    }类型解析数据  
******************************************************************************/
API_EXTERN int CChex_Update(void *CchexHandle, int *DevIdx, int *Type, char *Buff, int Len);


/*****************************************************************************
 * 上传指纹
 *   EmployeeId size  = 5
 *   长度见 #define FINGERPRINT_DATA_LEN
 *   FingerIdx : 指纹存储号(1-10) 人脸(11)
 * 信息返回类型:CCHEX_RET_ULFINGERPRT_TYPE
 * 信息解析结构:CCHEX_RET_ULFINGERPRT_STRU
*****************************************************************************/
API_EXTERN int CChex_UploadFingerPrint(void *CchexHandle, int DevIdx, unsigned char *EmployeeId, unsigned char FingerIdx, unsigned char *Data, unsigned int Data_len);
/*****************************************************************************
 * 下载指纹
 *   EmployeeId size  = 5
 *   长度见 #define FINGERPRINT_DATA_LEN
 *   FingerIdx : 指纹存储号(1-10) 人脸(11)
 * 信息返回类型:CCHEX_RET_DLFINGERPRT_TYPE
 * 信息解析结构:CCHEX_RET_DLFINGERPRT_STRU
*****************************************************************************/
API_EXTERN int CChex_DownloadFingerPrint(void *CchexHandle, int DevIdx, unsigned char *EmployeeId, unsigned char FingerIdx);
/*****************************************************************************
 * 上传指纹
 *   版本4:EmployeeId使用字符串ID size = 28
 *   长度见 #define FINGERPRINT_DATA_LEN
 *   FingerIdx : 指纹存储号(1-10) 人脸(11)
 * 信息返回类型:CCHEX_RET_ULFINGERPRT_TYPE
 * 信息解析结构:CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID
*****************************************************************************/
API_EXTERN int CChex_UploadFingerPrint_VER_4_NEWID(void *CchexHandle, int DevIdx, unsigned char *EmployeeId, unsigned char FingerIdx, unsigned char *Data, unsigned int Data_len);
/*****************************************************************************
 * 下载指纹
 *   EmployeeId size  = 5
 *   长度见 #define FINGERPRINT_DATA_LEN
 *   FingerIdx : 指纹存储号(1-10) 人脸(11)
 * 信息返回类型:CCHEX_RET_DLFINGERPRT_TYPE
 * 信息解析结构:CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID
*****************************************************************************/
API_EXTERN int CChex_DownloadFingerPrint_VER_4_NEWID(void *CchexHandle, int DevIdx, unsigned char *EmployeeId, unsigned char FingerIdx);



/*******************************************************************
 * 上传员工 一次最大上传12
 * 信息返回类型:CCHEX_RET_ULEMPLOYEE_INFO_TYPE CCHEX_RET_ULEMPLOYEE2_INFO_TYPE CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_DEL_EMPLOYEE_INFO_STRU
********************************************************************/
API_EXTERN int CChex_UploadEmployeeInfo(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE_INFO_STRU *EmployeeList, unsigned char EmployeeNum);
API_EXTERN int CChex_UploadEmployee2Info(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE2_INFO_STRU *EmployeeList, unsigned char EmployeeNum);
API_EXTERN int CChex_UploadEmployee2UnicodeInfo(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE2UNICODE_INFO_STRU *EmployeeList, unsigned char EmployeeNum);

/*******************************************************************
 * 上传员工 定制卡号长度7byte设备
 * 信息返回类型:CCHEX_RET_ULEMPLOYEE2UNICODE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_DEL_EMPLOYEE_INFO_STRU
********************************************************************/
API_EXTERN int CChex_UploadEmployee2UnicodeInfo_CardIdLen7(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU *EmployeeList, unsigned char EmployeeNum);

/****************************************************************
 * 定制Just  For   Bolid  Custom Person 定制设备
 * 信息返回类型:CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE
 * 信息解析结构:CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE
****************************************************************/
API_EXTERN int CChex_UploadEmployee2W2Info(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE2W2_INFO_STRU *EmployeeList, unsigned char EmployeeNum);

/****************************************************************
 * 下载员工 761 定制 
****************************************************************/
API_EXTERN int CChex_DownloadEmployeeInfo(void *CchexHandle, int DevIdx, unsigned int EmployeeCnt);

/****************************************************************
 * 下载人员 通用接口 :获取所有人员信息
 * 信息返回类型: CCHEX_RET_DLEMPLOYEE_INFO_TYPE 
 * 信息解析结构:协议类型,不同设备不同员工结构体:CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_W2    CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF
 *                                          CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID
****************************************************************/
API_EXTERN int CChex_ListPersonInfo(void *CchexHandle, int DevIdx);


/*******************************************************************
 * 上传文件:更新文件
 * 信息返回类型:CCHEX_RET_UPLOADFILE_TYPE
 * 信息解析结构:CCHEX_RET_UPLOADFILE_STRU
********************************************************************/
//FileType 0: firmware 1:pic 2: audio 3: language file
API_EXTERN int CChex_UploadFile(void *CchexHandle, int DevIdx, unsigned char FileType, char *FileName, char *Buff, int Len);


/*****************************************************************************
 * [NUM : 2.6]
 * 获取网络信息
 * 信息返回类型:CCHEX_RET_GETNETCFG_TYPE  
 * 信息解析结构:CCHEX_RET_GETNETCFG_STRU
******************************************************************************/
API_EXTERN int CChex_GetNetConfig(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * [NUM : 2.7]
 * 设置网络信息
 * 信息返回类型:CCHEX_RET_SETNETCFG_TYPE  
 * 信息解析结构:CCHEX_RET_SETNETCFG_STRU
******************************************************************************/
API_EXTERN int CChex_SetNetConfig(void *CchexHandle, int DevIdx, CCHEX_NETCFG_INFO_STRU *Config);



/*****************************************************************************
 * [NUM : 2.15]
 *下载 全部记录 
 * 日期时间为相距2000.01.02后的秒数。
 * 信息返回类型:CCHEX_RET_RECORD_INFO_TYPE
 * 信息解析结构:CCHEX_RET_RECORD_INFO_STRU
******************************************************************************/
API_EXTERN int CChex_DownloadAllRecords(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * [NUM : 2.15]
 * 下载全部新记录
 * 日期时间为相距2000.01.02后的秒数。
 * 信息返回类型:CCHEX_RET_GET_NEW_RECORD_INFO_TYPE
 * 信息解析结构:CCHEX_RET_RECORD_INFO_STRU  字符串员工号设备CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID
******************************************************************************/
API_EXTERN int CChex_DownloadAllNewRecords(void *CchexHandle, int DevIdx);

/*****************************************************************************
 * 上传 考勤记录 
 * 信息返回类型:CCHEX_RET_UPLOAD_RECORD_TYPE
 * 信息解析结构:CCHEX_RET_DEL_EMPLOYEE_INFO_STRU   CCHEX_RET_COMMON__WITH_EMPLOYEE_VER_4_NEWID
*****************************************************************************/
API_EXTERN int CChex_UploadRecord(void *CchexHandle, int DevIdx,CCHEX_UPLOAD_RECORD_INFO_STRU *Record);
API_EXTERN int CChex_UploadRecord_VER_4_NEWID(void *CchexHandle, int DevIdx,CCHEX_UPLOAD_RECORD_INFO_STRU_VER_4_NEWID *Record);


/*****************************************************************************
 *  下载口罩测温考勤全部记录  测温或口罩记录 (FACE7EI FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3T, FDEEP3M, FDEEP3TM,FD52M,FD52TM,FK20M,FK20TM)
 * 信息返回类型:CCHEX_RET_TM_ALL_RECORD_INFO_TYPE
 * 信息解析结构:CCHEX_RET_TM_RECORD_INFO_STRU
 *****************************************************************************/
API_EXTERN int CChex_TM_DownloadAllRecords(void *CchexHandle, int DevIdx);
/*****************************************************************************
 *  下载口罩测温考勤全部新记录  测温或口罩记录 (FACE7EI FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3T, FDEEP3M, FDEEP3TM,FD52M,FD52TM,FK20M,FK20TM)
 * 信息返回类型:CCHEX_RET_TM_NEW_RECORD_INFO_TYPE
 * 信息解析结构:CCHEX_RET_TM_RECORD_INFO_STRU
 *****************************************************************************/
API_EXTERN int CChex_TM_DownloadAllNewRecords(void *CchexHandle, int DevIdx);
/*****************************************************************************
 *  上传口罩测温考勤记录  测温或口罩记录 (FACE7EI FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3T, FDEEP3M, FDEEP3TM,FD52M,FD52TM,FK20M,FK20TM)
 * 信息返回类型:CCHEX_RET_TM_UPLOAD_RECORD_INFO_TYPE
 * 信息解析结构:CCHEX_RET_TM_UPLOAD_RECORD_STRU
 *****************************************************************************/
API_EXTERN int CChex_TM_UploadRecord(void *CchexHandle, int DevIdx,CCHEX_TM_UPLOAD_RECORD_INFO_STRU *Record);
/*****************************************************************************
 *  下载指定员工指定时间内口罩测温考勤记录  (FACE7EI FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3T, FDEEP3M, FDEEP3TM,FD52M,FD52TM,FK20M,FK20TM)
 * 信息返回类型:CCHEX_RET_TM_RECORD_BY_EMPLOYEE_TIME_TYPE
 * 信息解析结构:CCHEX_RET_TM_RECORD_INFO_STRU
 *****************************************************************************/
API_EXTERN int CChex_TM_DownloadRecordByEmployeeIdAndTime(void *CchexHandle, int DevIdx,CCHEX_GET_RECORD_INFO_BY_TIME *Param);


/*****************************************************************************
 * [NUM : 2.8]
 * 信息功能:读取指定索引短消息
 * Idx:索引号
 * 信息返回类型:CCHEX_RET_MSGGETBYIDX_UNICODE_INFO_TYPE   Seat定制:CCHEX_RET_MSGGETBYIDX_UNICODE_S_DATE_INFO_TYPE
 * 信息解析结构:CCHEX_MSGHEAD_INFO_STRU    CCHEX_MSGHEADUNICODE_INFO_STRU
 *     字符串员工号设备:CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID     CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID
*****************************************************************************/
API_EXTERN int CChex_MsgGetByIdx(void *CchexHandle, int DevIdx, unsigned char Idx);
/*****************************************************************************
 * [NUM : 2.9]
 * 信息功能:删除指定索引短消息
 * Idx:索引号
 * 信息返回类型:CCHEX_RET_MSGDELBYIDX_INFO_TYPE
 * 信息解析结构:CCHEX_RET_MSGDELBYIDX_UNICODE_STRU
*****************************************************************************/
API_EXTERN int CChex_MsgDelByIdx(void *CchexHandle, int DevIdx, unsigned char Idx);
/*****************************************************************************
 * [NUM : 2.11]
 * 信息功能:获取全部信息头
 * 信息返回类型:CCHEX_RET_MSGGETALLHEAD_INFO_TYPE CCHEX_RET_MSGGETALLHEADUNICODE_INFO_TYPE   Seat定制:CCHEX_RET_MSGGETALLHEADUNICODE_S_DATE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU    
*****************************************************************************/
API_EXTERN int CChex_MsgGetAllHead(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * [NUM : 2.10]
 * 信息功能:增加新信息
 * 信息返回类型:CCHEX_RET_MSGADDNEW_INFO_TYPE  CCHEX_RET_MSGADDNEW_UNICODE_INFO_TYPE   Seat定制:CCHEX_RET_MSGADDNEW_UNICODE_S_DATE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_MSGADDNEW_UNICODE_STRU
*****************************************************************************/
API_EXTERN int CChex_MsgAddNew(void *CchexHandle, int DevIdx, unsigned char *Data, int Len);


/*****************************************************************************
 * [NUM : 2.12]
 * 重启考勤机设备
 * 信息返回类型:CCHEX_RET_REBOOT_TYPE 
 * 信息解析结构:CCHEX_RET_REBOOT_STRU
*****************************************************************************/
API_EXTERN int CChex_RebootDevice(void *CchexHandle, int DevIdx);


/*****************************************************************************
 * [NUM : 2.13]
 * 设置系统时间: 年,月,日,时,分,秒
 * sample 2017 9 30 10 10 10    --> set time 2017/09/10 10:10:10
 * 信息返回类型:CCHEX_RET_SETTIME_TYPE
 * 信息解析结构:CCHEX_RET_SETTIME_STRU
*****************************************************************************/
API_EXTERN int CChex_SetTime(void *CchexHandle, int DevIdx, int Year, int Month, int Day, int Hour, int Min, int Sec);
/*****************************************************************************
 * 获取系统时间
 * 信息返回类型:CCHEX_RET_GETTIME_TYPE
 * 信息解析结构:CCHEX_MSG_GETTIME_STRU_EXT_INF
*****************************************************************************/
API_EXTERN int CChex_GetTime(void *CchexHandle, int DevIdx);


/*****************************************************************************
 * [NUM : 2.14]
 * 获取系统的SN号
 * 信息返回类型:CCHEX_RET_GET_SN_TYPE
 * 信息解析结构:CCHEX_RET_SN_STRU
*****************************************************************************/
API_EXTERN int CChex_GetSNConfig(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * 设置系统的SN号(SN唯一,设备默认禁止设置SN号)
 * 信息返回类型:CCHEX_RET_SET_SN_TYPE
 * 信息解析结构:CCHEX_RET_SN_STRU
*****************************************************************************/
API_EXTERN int CChex_SetSNConfig(void *CchexHandle, int DevIdx, unsigned char *sn);


/****************************************************************************
 * 761
****************************************************************************/
API_EXTERN int CChex_UploadEmployee3Info(void *CchexHandle, int DevIdx, CCHEX_EMPLOYEE3_INFO_STRU *EmployeeList, unsigned char EmployeeNum);
API_EXTERN int CChex_DownloadEmployee3Info(void *CchexHandle, int DevIdx, unsigned int EmployeeCnt);


/*****************************************************************************
 * 获取考勤机的基本配置参数
 * 信息返回类型:CCHEX_RET_GET_BASIC_CFG_TYPE
 * 信息解析结构:CCHEX_RET_GET_BASIC_CFG_STRU_EXT_INF
*****************************************************************************/
API_EXTERN int CChex_GetBasicConfigInfo(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * 设置考勤机的基本配置参数
 * 信息返回类型:CCHEX_RET_SET_BASIC_CFG_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
*****************************************************************************/
API_EXTERN int CChex_SetBasicConfigInfo(void *CchexHandle, int DevIdx, CCHEX_SET_BASIC_CFG_INFO_STRU_EXT_INF *config);


/**************************************************************************************************
 *修改或增加人员 3个版本  
 *          版本1 普通(员工ID为数字ID,不包含员工效时间)   
 *          版本2 Bolid定制(员工ID为数字ID,包含员工有效时间)   
 *          版本3 新通用版本(员工ID为字符串ID,包含员工有效时间)
 *          版本4 通用版本(员工ID为字符串ID,卡号长度为7byte)
 * 信息返回类型:CCHEX_RET_ULEMPLOYEE_INFO_TYPE   CCHEX_RET_ULEMPLOYEE2_INFO_TYPE CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE 
 *             CCHEX_RET_ULEMPLOYEE2_UNICODE_INFO_TYPE   CCHEX_RET_ULEMPLOYEE_VER_4_NEWID_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_WITH_EMPLOYEE_ID       CCHEX_RET_COMMON__WITH_EMPLOYEE_VER_4_NEWID
 *************************************************************************************************/
//版本 1
API_EXTERN int CChex_ModifyPersonInfo(void *CchexHandle, int DevIdx, CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF *EmployeeList, unsigned char EmployeeNum);
//版本 2
API_EXTERN int CChex_ModifyPersonW2Info(void *CchexHandle, int DevIdx, CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 *EmployeeList, unsigned char EmployeeNum);
//版本 3
API_EXTERN int CChex_ModifyPersonInfo_VER_4_NEWID(void *CchexHandle, int DevIdx, CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID *EmployeeList, unsigned char EmployeeNum);


/****************************************************************
 * 删除员工
 * Employee.backup   bit 0:fingerprint1,1:fingerprint2,3:passwork; 4:card; 0xFF all person info
 * 信息返回类型:CCHEX_RET_DEL_EMPLOYEE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_WITH_EMPLOYEE_ID     CCHEX_RET_COMMON__WITH_EMPLOYEE_VER_4_NEWID
****************************************************************/
API_EXTERN int CChex_DeletePersonInfo(void *CchexHandle, int DevIdx, CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF *Employee);
API_EXTERN int CChex_DeletePersonInfo_VER_4_NEWID(void *CchexHandle, int DevIdx, CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID *Employee);


/****************************************************************
 * 获取单个员工资料
 * 信息返回类型:CCHEX_RET_GET_ONE_EMPLOYEE_INFO_TYPE
 * 信息解析结构:CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4_NEWID
****************************************************************/
API_EXTERN int CChex_GetOnePersonInfo(void *CchexHandle, int DevIdx, CCHEX_GET_ONE_EMPLOYEE_INFO_STRU *Employee);
API_EXTERN int CChex_GetOnePersonInfo_VER_4_NEWID(void *CchexHandle, int DevIdx, CCHEX_GET_ONE_EMPLOYEE_INFO_STRU_ID_VER_4_NEWID *Employee);


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
API_EXTERN int CChex_DeleteRecordInfo(void *CchexHandle, int DevIdx, CCHEX_DEL_RECORD_OR_NEW_FLAG_INFO_STRU_EXT_INF *record);


/*****************************************************************************
 * 功能:初始化用户空间 
 * 信息返回类型:CCHEX_RET_INIT_USER_AREA_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CChex_InitUserArea(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * 功能:初始化系统 
 * 信息返回类型:CCHEX_RET_INIT_SYSTEM_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CChex_InitSystem(void *CchexHandle, int DevIdx);


/*****************************************************************************
 * 功能:获取基本配置2
 * 信息返回类型:CCHEX_RET_GET_BASIC_CFG2_TYPE
 * 信息解析结构:CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF
******************************************************************************/
API_EXTERN int CChex_GetBasicConfigInfo2(void *CchexHandle, int DevIdx);
/*****************************************************************************
 * 功能:设置基本配置2
 * 信息返回类型:CCHEX_RET_SET_BASIC_CFG2_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CChex_SetBasicConfigInfo2(void *CchexHandle, int DevIdx, CCHEX_SET_BASIC_CFG_INFO2_STRU_EXT_INF *Config);


/*****************************************************************************
 * 功能:获取时段信息
 * 信息返回类型:CCHEX_RET_GET_PERIOD_TIME_TYPE
 * 信息解析结构:CCHEX_GET_PERIOD_TIME_STRU_EXT_INF
******************************************************************************/
API_EXTERN int CChex_GetPeriodTime(void *CchexHandle, int DevIdx,unsigned char SerialNumbe);//(SerialNumbe==(1--32))
/*****************************************************************************
 * 功能:设置时段信息
 * 信息返回类型:CCHEX_RET_SET_PERIOD_TIME_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CChex_SetPeriodTime(void *CchexHandle, int DevIdx, CCHEX_SET_PERIOD_TIME_STRU_EXT_INF *Config);


/*****************************************************************************
 * 功能:获取组信息
 * 信息返回类型:CCHEX_RET_GET_TEAM_INFO_TYPE
 * 信息解析结构:CCHEX_GET_TEAM_INFO_STRU_EXT_INF
******************************************************************************/
API_EXTERN int CChex_GetTeamInfo(void *CchexHandle, int DevIdx,unsigned char TeamNumbe);//(TeamNumbe==(2--16))
/*****************************************************************************
 * 功能:设置组信息
 * 信息返回类型:CCHEX_RET_SET_TEAM_INFO_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CChex_SetTeamInfo(void *CchexHandle, int DevIdx, CCHEX_SET_TEAM_INFO_STRU_EXT_INF *Config);//(TeamNumbe==(2--16))


/*****************************************************************************
 * 功能:在线登记指纹
 * 信息返回类型:CCHEX_RET_ADD_FINGERPRINT_ONLINE_TYPE
 * 信息解析结构:CCHEX_RET_DLFINGERPRT_STRU  CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID
 * @brief 
 * 在线登记指纹
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param Param         相关参数(注:在线登记时,需使用一个临时用户登记,登记完成后该临时用户将会被"删除",请输入一个非正式使用员工号)
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CCHex_AddFingerprintOnline(void *CchexHandle, int DevIdx, CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF *Param);
API_EXTERN int CCHex_AddFingerprintOnline_VER_4_NEWID(void *CchexHandle, int DevIdx, CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF_ID_VER_4_NEWID *Param);


/*****************************************************************************
 * 功能:强制开锁
 * CCHEX_FORCED_UNLOCK_STRU_EXT_INF *Param = NULL  ;  just  Customized version :"Panasonic" use Param,
 * 非定制版本传入NULL
 * 信息返回类型:CCHEX_RET_FORCED_UNLOCK_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
******************************************************************************/
API_EXTERN int CCHex_ForcedUnlock(void *CchexHandle, int DevIdx,CCHEX_FORCED_UNLOCK_STRU_EXT_INF *Param);


/***********************************
 * 功能 : 获取厂商信息码
 * 补充说明 : ANSI版本    信息码长度  10  UNICODE版本      信息码  20
 * 信息返回类型:CCHEX_RET_GET_INFOMATION_CODE_TYPE
 * 信息解析结构:CCHEX_RET_GET_INFOMATOIN_CODE_STRU
************************************/
API_EXTERN int CChex_GetInfomationCode(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置厂商信息码
 * 补充说明 : ANSI版本    信息码长度  10  UNICODE版本      信息码  20
 * 信息返回类型:CCHEX_RET_SET_INFOMATION_CODE_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetInfomationCode(void *CchexHandle, int DevIdx,unsigned char *Data, unsigned int DataLen); //Data为 10 长度  或者 20长度的 字符数组


/***********************************
 * 功能 : 获取打铃信息
 * 参数 : FlagWeek 如果要设定星期一到星期五设定某时间段打铃，那么参数d的值就等于00111110=62
 *                  星期标志FlagWeek(用二进制0000000分别表示：六五四三二一日)
 * 信息返回类型:CCHEX_RET_GET_BELL_INFO_TYPE
 * 信息解析结构:CCHEX_RET_GET_BELL_INFO_STRU
************************************/
API_EXTERN int CChex_GetBellInfo(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置打铃信息
 * 参数 : FlagWeek 如果要设定星期一到星期五设定某时间段打铃，那么参数d的值就等于00111110=62
 *                  星期标志FlagWeek(用二进制0000000分别表示：六五四三二一日)
 * 信息返回类型:CCHEX_RET_SET_BELL_INFO_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetBellInfo(void *CchexHandle, int DevIdx,unsigned char BellTimeNum,unsigned char Hour, unsigned char Min,unsigned char FlagWeek);


/***********************************
 * 功能 : 获取自定义考勤状态表
 * 补充说明 : ANSI版本    Data长度:  161  UNICODE版本      Data长度:  321
 * 信息返回类型:CCHEX_RET_GET_USER_ATTENDANCE_STATUS_TYPE
 * 信息解析结构:CCHEX_RET_GET_USER_ATTENDANCE_STATUS_STRU
************************************/
API_EXTERN int CChex_GetUserAttendanceStatusInfo(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置自定义考勤状态表
 * 补充说明 : ANSI版本    Data长度:  161  UNICODE版本      Data长度:  321
 * 信息返回类型:CCHEX_RET_SET_USER_ATTENDANCE_STATUS_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetUserAttendanceStatusInfo(void *CchexHandle, int DevIdx,CCHEX_SET_USER_ATTENDANCE_STATUS_STRU *Param);


/***********************************
 * 功能 : 清除管理员标志
 * 信息返回类型:CCHEX_RET_CLEAR_ADMINISTRAT_FLAG_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_ClearAdministratFlag(void *CchexHandle, int DevIdx);


/***********************************
 * 功能 : 取特殊状态    
 * 信息返回类型:CCHEX_RET_GET_SPECIAL_STATUS_TYPE
 * 信息解析结构:CCHEX_RET_GET_SPECIAL_STATUS_STRU
************************************/
API_EXTERN int CChex_GetSpecialStatus(void *CchexHandle, int DevIdx);


/***********************************
 * 功能 : 读取管理卡号/管理密码        T5专用
 * 参数 :data[13]  : 如果机型为T5A，则为:     DATA	添加卡号	删除卡号	胁迫卡号	特殊信息
                                            Byte	 1-4	    5-8	      9-12	      13
                                            特殊信息定义如下:
        	                                位0：添加卡号
        	                                位1：删除卡号
                                            位2：胁迫卡号
                   : 如果机型为T50，则为:     DATA	管理密码长度+管理密码	    保留
                                            Byte	       1-3	             4-13
                                            管理密码长度 = Byte(1) >> 4
 * 信息返回类型:CCHEX_RET_GET_ADMIN_CARD_PWD_TYPE
 * 信息解析结构:CCHEX_RET_GET_ADMIN_CARDNUM_PWD_STRU
************************************/
API_EXTERN int CChex_GetAdminCardnumberPassword(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置管理卡号/管理密码        T5专用
 * 参数 :data[13]  : 如果机型为T5A，则为:     DATA	添加卡号	删除卡号	胁迫卡号	特殊信息
                                            Byte	 1-4	    5-8	      9-12	      13
                                            特殊信息定义如下:
        	                                位0：添加卡号
        	                                位1：删除卡号
                                            位2：胁迫卡号
                   : 如果机型为T50，则为:     DATA	管理密码长度+管理密码	    保留
                                            Byte	       1-3	             4-13
                                            管理密码长度 = Byte(1) >> 4
 * 信息返回类型:CCHEX_RET_SET_ADMIN_CARD_PWD_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetAdminCardnumberPassword(void *CchexHandle, int DevIdx,unsigned char *Data,unsigned int DataLen); //Data[13] DataLen == 13


/***********************************
 * 功能 : 读取夏令时参数
 * 信息返回类型:CCHEX_RET_GET_DST_PARAM_TYPE
 * 信息解析结构:CCHEX_RET_GET_DST_PARAM_STRU
************************************/
API_EXTERN int CChex_GetDSTParam(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置夏令时参数
 * 信息返回类型:CCHEX_RET_SET_DST_PARAM_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetDSTParam(void *CchexHandle, int DevIdx,CCHEX_SET_DST_PARAM_STRU *Param);


/***********************************
 * 功能 : 获取机器扩展信息码
 * 信息返回类型:CCHEX_RET_GET_DEV_EXT_INFO_TYPE
 * 信息解析结构:CCHEX_RET_GET_DEV_EXT_INFO_STRU
************************************/
API_EXTERN int CChex_GetDevExtInfo(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置机器扩展信息码
 * 信息返回类型:CCHEX_RET_SET_DEV_EXT_INFO_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetDevExtInfo(void *CchexHandle, int DevIdx,CCHEX_SET_DEV_EXT_INFO_STRU *Param);


/***********************************
 * 功能 : 获取考勤机配置信息3	
 * 信息返回类型:CCHEX_RET_GET_BASIC_CFG3_TYPE
 * 信息解析结构:CCHEX_RET_GET_BASIC_CFG_INFO3_STRU
************************************/
API_EXTERN int CChex_GetBasicConfigInfo3(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置考勤机配置信息3	
 * 信息返回类型:CCHEX_RET_SET_BASIC_CFG3_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetBasicConfigInfo3(void *CchexHandle, int DevIdx, CCHEX_SET_BASIC_CFG_INFO3_STRU *Config);


/***********************************
 * 功能 : 连接认证	CMD：0x04   虹膜设备用户 密码，其他设备，不验证用户名，只验证 通讯密码。
 * 信息返回类型:CCHEX_RET_CONNECTION_AUTHENTICATION_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_ConnectionAuthentication(void *CchexHandle, int DevIdx,CCHEX_CONNECTION_AUTHENTICATION_STRU *Param);


/***********************************
 * 功能 : 按员工工号和时间获取考勤记录数量
 * 信息返回类型:CCHEX_RET_GET_RECORD_NUMBER_TYPE
 * 信息解析结构:CCHEX_RET_GET_RECORD_NUMBER_STRU
************************************/
API_EXTERN int CChex_GetRecordNumByEmployeeIdAndTime(void *CchexHandle, int DevIdx,CCHEX_GET_RECORD_INFO_BY_TIME *Param);
API_EXTERN int CChex_GetRecordNumByEmployeeIdAndTime_VER_4_NEWID(void *CchexHandle, int DevIdx,CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID *Param);
/***********************************
 * 功能 : 按员工工号和时间下载考勤记录
 * 信息返回类型:CCHEX_RET_GET_RECORD_BY_EMPLOYEE_TIME_TYPE 
 * 信息解析结构:CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU     CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID
************************************/
API_EXTERN int CChex_DownloadRecordByEmployeeIdAndTime(void *CchexHandle, int DevIdx,CCHEX_GET_RECORD_INFO_BY_TIME *Param);
API_EXTERN int CChex_DownloadRecordByEmployeeIdAndTime_VER_4_NEWID(void *CchexHandle, int DevIdx,CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID *Param);


/***********************************
 * 功能 : 获取设备人员和记录信息	
 * 信息返回类型:CCHEX_RET_DEV_STATUS_TYPE
 * 信息解析结构:CCHEX_RET_DEV_STATUS_STRU
************************************/
API_EXTERN int CChex_GetRecordInfoStatus(void *CchexHandle, int DevIdx);


/***********************************
 * 功能 : 获取URL
 * 信息返回类型:CCHEX_RET_GET_URL_TYPE
 * 信息解析结构:CCHEX_RET_GET_URL_STRU
************************************/
API_EXTERN int CChex_GetServiceURL(void *CchexHandle,int DevIdx);
/***********************************
 * 功能 : 设置URL
 * 信息返回类型:CCHEX_RET_SET_URL_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetServiceURL(void *CchexHandle,int DevIdx,CCHEX_SET_URL_STRU *Param);        //lengthMax of Buff <=104byte


/***********************************
 * 功能 : 获取状态切换信息  （p_dev->DevTypeFlag & 0x200000）
 * 信息返回类型:CCHEX_RET_GET_STATUS_SWITCH_TYPE
 * 信息解析结构:CCHEX_RET_GET_STATUS_SWITCH_STRU
************************************/
API_EXTERN int CChex_GetStatusSwitch(void *CchexHandle, int DevIdx,unsigned char GroupId);                  //(GroupId==(1--16))
/***********************************
 * 功能 : 设置状态切换信息  （p_dev->DevTypeFlag & 0x200000）
 * 信息返回类型:CCHEX_RET_SET_STATUS_SWITCH_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetStatusSwitch(void *CchexHandle, int DevIdx, CCHEX_SET_STATUS_SWITCH_STRU *Param);


/***********************************
 * 功能 : 获取设置状态切换信息  （p_dev->DevTypeFlag & 0x100000）
 * 信息返回类型:CCHEX_RET_GET_STATUS_SWITCH_EXT_TYPE
 * 信息解析结构:CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT
************************************/
API_EXTERN int CChex_GetStatusSwitch_EXT(void *CchexHandle, int DevIdx,unsigned char FlagWeek);                  //flag_week bit0-6:(0000000:6,5,4,3,2,1,sun     1:select)
/***********************************
 * 功能 : 获取设置状态切换信息  （p_dev->DevTypeFlag & 0x100000）
 * 信息返回类型:CCHEX_RET_SET_STATUS_SWITCH_EXT_TYPE
 * 信息解析结构:CCHEX_RET_COMMON_STRU
************************************/
API_EXTERN int CChex_SetStatusSwitch_EXT(void *CchexHandle, int DevIdx, CCHEX_SET_STATUS_SWITCH_STRU_EXT *Param);


/***********************************
 * 功能 : 获取照片文件总数      现支持型号:OA1000
 * 信息返回类型:CCHEX_RET_PICTURE_GET_TOTAL_NUMBER_TYPE
 * 信息解析结构:CCHEX_RET_GET_PICTURE_NUMBER_STRU
************************************/
API_EXTERN int CChex_GetPictureNumber(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 获取照片文件头信息    现支持型号:OA1000
 * 信息返回类型:CCHEX_RET_PICTURE_GET_ALL_HEAD_TYPE
 * 信息解析结构:CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU    CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID
************************************/
API_EXTERN int CChex_GetPictureAllHeadInfo(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 获取指定照片文件      现支持型号:OA1000
 * 信息返回类型:CCHEX_RET_PICTURE_GET_DATA_BY_EID_TIME_TYPE
 * 信息解析结构:CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME  CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID
************************************/
API_EXTERN int CChex_GetPictureByEmployeeIdAndTime(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_EID_AND_TIME *Param);
API_EXTERN int CChex_GetPictureByEmployeeIdAndTime_VER_4_NEWID(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID *Param);
/***********************************
 * 功能 : 删除指定照片文件      现支持型号:OA1000
 * 信息返回类型:CCHEX_RET_PICTURE_DEL_DATA_BY_EID_TIME_TYPE
 * 信息解析结构:CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME    CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID
************************************/
API_EXTERN int CChex_DelPictureByEmployeeIdAndTime(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_EID_AND_TIME *Param);
API_EXTERN int CChex_DelPictureByEmployeeIdAndTime_VER_4_NEWID(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID *Param);





/**********************************************************************人脸图片模块开始*****************************************************************/
/***********************************
 * 功能 : 获取测温记录数     Type:0-所有类型,10-测温正常,20-测温异常      现支持型号:FACE7T    FACE7TM  FDEEP5T  FDEEP5TM    FDEEP3T  FDEEP3TM FD52TM FK20TM
 * 信息返回类型:CCHEX_RET_GET_T_RECORD_NUMBER_TYPE
 * 信息解析结构:CCHEX_RET_GET_T_RECORD_NUMBER_STRU
 * @brief 
 * 获取测温记录数
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param RecordTpye    0-所有类型,10-测温正常,20-测温异常 
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CChex_GetTRecordNumberByType(void *CchexHandle, int DevIdx,int RecordTpye);
/***********************************
 * 功能 : 根据测温类型下载记录                                          现支持型号:FACE7T    FACE7TM  FDEEP5T  FDEEP5TM     FDEEP3T  FDEEP3TM FD52TM FK20TM
 * 信息返回类型:CCHEX_RET_GET_T_RECORD_TYPE
 * 信息解析结构:CCHEX_RET_GET_T_RECORD_STRU
 * @brief 
 * 根据测温类型下载记录
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param RecordTpye    0-所有类型,10-测温正常,20-测温异常
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CChex_GetTRecordByType(void *CchexHandle, int DevIdx,int RecordTpye);
/***********************************
 * 功能 : 获取指定测温记录ID照片文件                                    现支持型号:FACE7T    FACE7TM  FDEEP5T  FDEEP5TM     FDEEP3T  FDEEP3TM FD52TM FK20TM
 * 信息返回类型:CCHEX_RET_GET_T_PICTURE_BY_RECORD_ID_TYPE
 * 信息解析结构:CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU
************************************/
API_EXTERN int CChex_GetPictureByTRecordIdType(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_RECORD_ID_STRU *Param);
/***********************************
 * 功能 : 删除指定测温记录ID照片文件                                    现支持型号:FACE7T    FACE7TM  FDEEP5T  FDEEP5TM     FDEEP3T  FDEEP3TM FD52TM FK20TM
 * 信息返回类型:CCHEX_RET_DEL_T_PICTURE_BY_RECORD_ID_TYPE
 * 信息解析结构:CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU
************************************/
API_EXTERN int CChex_DelPictureByTRecordIdType(void *CchexHandle, int DevIdx,CCHEX_PICTURE_BY_RECORD_ID_STRU *Param);

/***********************************
 * 功能 : 图片人脸模板-下载                                  
 * 现支持型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM,FD52,FD52M,FD52TM,FACE7PRO,FK20,FK20M,FK20TM PASS7PT PASS7PM PASS7PTM 
 * 信息返回类型:CCHEX_RET_DOWNLOAD_FACE_PICTURE_MODULE_TYPE
 * 信息解析结构:CCHEX_DOWNLOAD_FACE_PICTURE_MODULE
 * @brief 
 * 图片人脸模板-下载   
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param Param         相关参数
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CChex_DownloadFacePictureModule(void *CchexHandle, int DevIdx,CCHEX_FACE_PICTURE_MODULE_STRU *Param);

/***********************************
 * 功能 : 图片人脸模板-上传                                  
 * 现支持型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM,FD52,FD52M,FD52TM,FACE7PRO,FK20 FK20M FK20TM PASS7PT PASS7PM PASS7PTM 
 * PictureBuff, BuffLen:MAX: <= 512000              The image format must be JPG
 * 信息返回类型:CCHEX_RET_UPLOAD_FACE_PICTURE_MODULE_TYPE
 * 信息解析结构:CCHEX_UPLOAD_FACE_PICTURE_MODULE
 * @brief 
 * 图片人脸模板-上传   
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param Param         相关参数
 * @param PictureBuff   人脸模块或人脸图片数据
 * @param BuffLen       人脸模块或人脸图片数据长度
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CChex_UploadFacePictureModule(void *CchexHandle, int DevIdx,CCHEX_FACE_PICTURE_MODULE_STRU *Param,char*PictureBuff,int BuffLen);
/*****************************************************************************
 * 功能:在线登记指纹 人脸模块
 * 
 * 设备型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM,FD52,FD52M,FD52TM,FACE7PRO FK20 FK20M FK20TM PASS7PT PASS7PM PASS7PTM 
 * 信息返回类型:CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE
 * 信息解析结构:CCHEX_DOWNLOAD_FACE_PICTURE_MODULE  
 * @brief 
 * 在线登记指纹
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param Param         相关参数(注:在线登记时,需使用一个临时用户登记,登记完成后该临时用户将会被"删除",请输入一个非正式使用员工号)
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
API_EXTERN int CCHex_AddFingerprintOnline_FacePicture(void *CchexHandle, int DevIdx, CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF *Param);
/**********************************************************************人脸图片模块结束*************************************************************/




/***********************************
 * 功能 : 更新固件校验状态   1：校验完成2：校验成功。即可重启升级
 * 信息返回类型:CCHEX_RET_UPDATEFILE_STATUS_TYPE
 * 信息解析结构:CCHEX_RET_UPDATEFILE_STATUS
************************************/
API_EXTERN int CChex_UpdateDevStatus(void *CchexHandle, int DevIdx);

/***********************************
 * 功能 : 获取设备机器号
 * 信息返回类型:CCHEX_RET_GET_MACHINE_ID_TYPE
 * 信息解析结构:CCHEX_RET_GET_MACHINE_ID_STRU
************************************/
API_EXTERN int CChex_GetMachineId(void *CchexHandle, int DevIdx);
/***********************************
 * 功能 : 设置设备机器号
 * 信息返回类型:CCHEX_RET_SET_MACHINE_ID_TYPE
 * 信息解析结构:CCHEX_RET_SET_MACHINE_ID_STRU
************************************/
API_EXTERN int CChex_SetMachineId(void *CchexHandle, int DevIdx,unsigned int MachineId);



/*****************************************************************************
 * Custom For Bolid   BasiConfigInfo5 定制
 * 信息返回类型:  
 * 信息解析结构:
******************************************************************************/
API_EXTERN int CChex_GetBasicConfigInfo5(void *CchexHandle,int DevIdx);
API_EXTERN int CChex_SetBasicConfigInfo5(void *CchexHandle,int DevIdx,CCHEX_SET_BASIC_CFG_INFO5_STRU*Config);
API_EXTERN int CChex_GetCardNo(void *CchexHandle,int DevIdx);
API_EXTERN int CChex_SetDevCurrentStatus(void *CchexHandle,int DevIdx,CCHEX_SET_DEV_CURRENT_STATUS_STRU*Config);



/****************************************************************************
 * LOG record Manage
 * 信息返回类型:CCHEX_RET_MANAGE_LOG_RECORD_TYPE
 * 信息解析结构:CCHEX_RET_MANAGE_LOG_RECORD
****************************************************************************/
API_EXTERN int CChex_ManageLogRecord(void *CchexHandle,int DevIdx,CCHEX_MANAGE_LOG_RECORD *Param);


/*****************************************************************************
 * UDP搜索设备
 * 信息返回类型:CCHEX_RET_UDP_SEARCH_DEV_TYPE
 * 信息解析结构:CCHEX_UDP_SEARCH_STRU_EXT_INF
******************************************************************************/
API_EXTERN int CCHex_Udp_Search_Dev(void *CchexHandle);
/*****************************************************************************
 * UDP修改设备
 * 信息返回类型:CCHEX_RET_UDP_SET_DEV_CONFIG_TYPE
 * 信息解析结构:CCHEX_RET_SET_DEV_CONFIG_STRU_EXT_INF
******************************************************************************/
API_EXTERN int CCHex_Udp_Set_Dev_Config(void *CchexHandle, CCHEX_UDP_SET_DEV_CONFIG_STRU_EXT_INF *Config);//Config->DevHardwareType = 0:Dev without DNS; = 1:Dev has DNS;


/*****************************************************************************
 * Client: connect   returned  =  1    Cmd Ok        = -1  Cmd Fail
 * 信息返回类型:成功: CCHEX_RET_DEV_LOGIN_TYPE  失败:CCHEX_RET_CLINECT_CONNECT_FAIL_TYPE   
 * 信息解析结构:成功: CCHEX_RET_DEV_LOGIN_STRU  失败:CCHEX_RET_DEV_CONNECT_STRU
******************************************************************************/
API_EXTERN int CCHex_ClientConnect(void *CchexHandle, char *Ip, int Port);
/*****************************************************************************
 * Client: connect   returned  =  1    Cmd Ok        = -1  Cmd Fail
 * 信息返回类型:CCHEX_RET_DEV_LOGOUT_TYPE   
 * 信息解析结构:CCHEX_RET_DEV_LOGOUT_STRU
******************************************************************************/
API_EXTERN int CCHex_ClientDisconnect(void *CchexHandle,int DevIdx);








/*****************************************************************************
 * SAC822   门禁控制器 
******************************************************************************/
/****************************************************************
 1:*SAC 下载信息,获取配置接口 CChex_SAC_Download_Common
 2:*SAC 上传信息,设置配置接口 CChex_SAC_Upload_Common
 3:*SAC 删除信息,对配置无效 CChex_SAC_Delete_Common
 4:*SAC 初始化信息和配置 CChex_SAC_Init_Common
         CchexHandle:句柄   DevIdx:考勤机的设备索引  DataCount:上传和删除信息体数量 DataBuff:上传和删除的具体数据 DataLen:上传和删除的数据长度
*****************************************************************/
API_EXTERN int CChex_SAC_Download_Common(void *CchexHandle, int DevIdx,unsigned int CmdPri);
API_EXTERN int CChex_SAC_Upload_Common(void *CchexHandle, int DevIdx,unsigned int CmdPri,unsigned int DataCount,char *DataBuff,unsigned int DataLen);
API_EXTERN int CChex_SAC_Delete_Common(void *CchexHandle, int DevIdx,unsigned int CmdPri,unsigned int DataCount,char *DataBuff,unsigned int DataLen);
API_EXTERN int CChex_SAC_Init_Common(void *CchexHandle, int DevIdx,unsigned int CmdPri);


#ifdef __cplusplus
}
#endif

#endif
