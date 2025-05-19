using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AnvizDemo
{
    class AnvizNew
    {
        public enum CustomType : int
        {
            ANVIZ_CUSTOM_EMPLOYEE_FOR_DR_ADD_NAME2 = 0x10,
            ANVIZ_CUSTOM_EMPLOYEE_FOR_W2_ADD_TIME     = 0x20,
            DEV_TYPE_VER_4_NEWID = 0x40,
            ANVIZ_CUSTOM_MESSAGE_FOR_SEATS_200 = 0x400,
            DEV_TYPE_FLAG_CARDNO_BYTE_7 = 0x1000000,
        };
        public enum MODE_FP_Type : int
        {
            DEV_TYPE_FLAG_FP_LEN_338 = 0x1000,  //通用指纹设备
            //DEV_TYPE_FLAG_FP_LEN_1200 = 0x2000,  //旧固件IRIS2000设备
            DEV_TYPE_FLAG_FP_LEN_2400 = 0x2000,  //新固件IRIS2000设备
            DEV_TYPE_FLAG_FP_LEN_2048 = 0x4000,  //OA1000PU设备
            DEV_TYPE_FLAG_FP_LEN_6144 = 0x8000,  //OA1000PM
            //DEV_TYPE_FLAG_FP_LEN_10240 = 0x10000, //保留机型
            DEV_TYPE_FLAG_FP_LEN_2052 = 0x10000, //FDEEP5T
            DEV_TYPE_FLAG_FACEPASS_15360 = 0x20000, //FACE7
            DEV_TYPE_FLAG_FACEPASS_2056 = 0x40000, //FACE7FAI
            DEV_TYPE_FLAG_FACEPASS_1028 = 0x80000, //FDEEP5
            DEV_TYPE_FLAG_FACEPASS_PICTURE = 0x800000, //支持人脸图片数据模块 (FDEEP5 FDEEP5T FDEEP5M FDEEP5TM,FDEEP3 FDEEP3T FDEEP3M FDEEP3TM)
        };
        public enum MODE_Dev_Type : int
        {
            DEV_TYPE_FLAG_RECORD_COMMON  =   0x100000, //通用记录
            DEV_TYPE_FLAG_RECORD_TM = 0x200000, //测温或口罩记录 (FACE7T FACE7M FACE7TM  FDEEP5T, FDEEP5M, FDEEP5TM  FDEEP3T, FDEEP3M, FDEEP3TM ,PASS7PT PASS7PM PASS7PTM)
            DEV_TYPE_FLAG_RECORD_TEMPERATURE_T = 0x400000, //支持获取测温记录图片 (FACE7TM  FACE7T FDEEP5T FDEEP5TM FDEEP3T FDEEP3TM PASS7PT PASS7PTM)
        };
        public enum MessageType : int
        {
            DEV_TYPE_FLAG_MSG_ASCII_48 = 0x100,
            DEV_TYPE_FLAG_MSG_UNICODE_96 = 0x200,
        };
        public enum EmployeeType : int
        {
            DEV_TYPE_FLAG_MSG_ASCII_32 = 0x02,
            DEV_TYPE_FLAG_MSG_UNICODE_40 = 0x04,
        };
        public enum NetCardType : int
        {
            NETCARD_WITHOUT_DNS = 63,
            NETCARD_WITH_DNS = 167,
            NETCARD_TWO_CARD = 102,
        };
        public enum MsgType : int
        {
            CCHEX_RET_RECORD_INFO_TYPE              = 1,
            CCHEX_RET_DEV_LOGIN_TYPE,
            CCHEX_RET_DEV_LOGOUT_TYPE, 
            CCHEX_RET_DLFINGERPRT_TYPE              = 4,
            CCHEX_RET_ULFINGERPRT_TYPE              = 5,
            CCHEX_RET_ULEMPLOYEE_INFO_TYPE          = 6,
            CCHEX_RET_ULEMPLOYEE2_INFO_TYPE         = 7,
            CCHEX_RET_ULEMPLOYEE2_UNICODE_INFO_TYPE = 8,
            CCHEX_RET_LIST_PERSON_INFO_TYPE         = 9,
            CCHEX_RET_MSGGETBYIDX_INFO_TYPE         = 12,
            CCHEX_RET_MSGGETBYIDX_UNICODE_INFO_TYPE = 13,
            CCHEX_RET_MSGADDNEW_INFO_TYPE           = 14,
            CCHEX_RET_MSGADDNEW_UNICODE_INFO_TYPE   = 15,
            CCHEX_RET_MSGDELBYIDX_INFO_TYPE         = 16,
            CCHEX_RET_MSGGETALLHEAD_INFO_TYPE       = 17,

            CCHEX_RET_REBOOT_TYPE                   = 18,
            CCHEX_RET_DEV_STATUS_TYPE               = 19,
            CCHEX_RET_MSGGETALLHEADUNICODE_INFO_TYPE= 20,
            CCHEX_RET_SETTIME_TYPE                  = 21,
            CCHEX_RET_UPLOADFILE_TYPE               = 22,// = 22
            CCHEX_RET_GETNETCFG_TYPE                = 23,
            CCHEX_RET_SETNETCFG_TYPE                = 24,
            CCHEX_RET_GET_SN_TYPE                   = 25,
            CCHEX_RET_SET_SN_TYPE                   = 26,
            CCHEX_RET_DLEMPLOYEE_3_TYPE             = 27, // 761
            CCHEX_RET_ULEMPLOYEE_3_TYPE             = 28, // 761
            CCHEX_RET_GET_BASIC_CFG_TYPE            = 29,
            CCHEX_RET_SET_BASIC_CFG_TYPE            = 30,
            CCHEX_RET_DEL_PERSON_INFO_TYPE          = 31,
            CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE  = 33,
            CCHEX_RET_MSGGETBYIDX_UNICODE_S_DATE_INFO_TYPE      = 34,
            CCHEX_RET_MSGADDNEW_UNICODE_S_DATE_INFO_TYPE        = 35,
            CCHEX_RET_MSGGETALLHEADUNICODE_S_DATE_INFO_TYPE     = 36,


            CCHEX_RET_GET_BASIC_CFG2_TYPE           = 37,
            CCHEX_RET_SET_BASIC_CFG2_TYPE           = 38,
            CCHEX_RET_GETTIME_TYPE                  = 39,
            CCHEX_RET_INIT_USER_AREA_TYPE           = 40,
            CCHEX_RET_INIT_SYSTEM_TYPE              = 41,
            CCHEX_RET_GET_PERIOD_TIME_TYPE          = 42,
            CCHEX_RET_SET_PERIOD_TIME_TYPE          = 43,
            CCHEX_RET_GET_TEAM_INFO_TYPE            = 44,
            CCHEX_RET_SET_TEAM_INFO_TYPE            = 45,
            CCHEX_RET_ADD_FINGERPRINT_ONLINE_TYPE   = 46,
            CCHEX_RET_FORCED_UNLOCK_TYPE            = 47,
            CCHEX_RET_UDP_SEARCH_DEV_TYPE           = 48,
            CCHEX_RET_UDP_SET_DEV_CONFIG_TYPE       = 49,


            //

            CCHEX_RET_GET_INFOMATION_CODE_TYPE      = 50,
            CCHEX_RET_SET_INFOMATION_CODE_TYPE      = 51,
            CCHEX_RET_GET_BELL_INFO_TYPE            = 52,
            CCHEX_RET_SET_BELL_INFO_TYPE            = 53,
            CCHEX_RET_LIVE_SEND_ATTENDANCE_TYPE     = 54,
            CCHEX_RET_GET_USER_ATTENDANCE_STATUS_TYPE   = 55,
            CCHEX_RET_SET_USER_ATTENDANCE_STATUS_TYPE   = 56,
            CCHEX_RET_CLEAR_ADMINISTRAT_FLAG_TYPE   = 57,
            CCHEX_RET_GET_SPECIAL_STATUS_TYPE       = 58,
            CCHEX_RET_GET_ADMIN_CARD_PWD_TYPE       = 59,
            CCHEX_RET_SET_ADMIN_CARD_PWD_TYPE       = 60,
            CCHEX_RET_GET_DST_PARAM_TYPE            = 61,
            CCHEX_RET_SET_DST_PARAM_TYPE            = 62,
            CCHEX_RET_GET_DEV_EXT_INFO_TYPE         = 63,
            CCHEX_RET_SET_DEV_EXT_INFO_TYPE         = 64,
            CCHEX_RET_GET_BASIC_CFG3_TYPE           = 65,
            CCHEX_RET_SET_BASIC_CFG3_TYPE           = 66,
            CCHEX_RET_CONNECTION_AUTHENTICATION_TYPE    = 67,
            CCHEX_RET_GET_RECORD_NUMBER_TYPE            = 68,
            CCHEX_RET_GET_RECORD_BY_EMPLOYEE_TIME_TYPE  = 69,

            CCHEX_RET_GET_RECORD_INFO_STATUS_TYPE   = 70,
            CCHEX_RET_GET_NEW_RECORD_INFO_TYPE      = 71,

            CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE       = 72,
            CCHEX_RET_GET_BASIC_CFG5_TYPE           = 73,
            CCHEX_RET_SET_BASIC_CFG5_TYPE           = 74,
            CCHEX_RET_GET_CARD_ID_TYPE              = 75,
            CCHEX_RET_SET_DEV_CURRENT_STATUS_TYPE   = 76,
            CCHEX_RET_GET_URL_TYPE                  = 77,
            CCHEX_RET_SET_URL_TYPE                  = 78,
            CCHEX_RET_GET_STATUS_SWITCH_TYPE        = 79,
            CCHEX_RET_SET_STATUS_SWITCH_TYPE        = 80,
            CCHEX_RET_GET_STATUS_SWITCH_EXT_TYPE    = 81,
            CCHEX_RET_SET_STATUS_SWITCH_EXT_TYPE    = 82,
            CCHEX_RET_UPDATEFILE_STATUS_TYPE        = 83,

            CCHEX_RET_GET_MACHINE_ID_TYPE           = 84,
            CCHEX_RET_SET_MACHINE_ID_TYPE           = 85,

            CCHEX_RET_GET_MACHINE_TYPE_TYPE         = 86,

            CCHEX_RET_UPLOAD_RECORD_TYPE            = 87,

            CCHEX_RET_GET_ONE_EMPLOYEE_INFO_TYPE    = 88,

            CCHEX_RET_ULEMPLOYEE_VER_4_NEWID_TYPE   = 89,

            CCHEX_RET_MANAGE_LOG_RECORD_TYPE        = 90,

            CCHEX_RET_PICTURE_GET_TOTAL_NUMBER_TYPE = 91,
            CCHEX_RET_PICTURE_GET_ALL_HEAD_TYPE = 92,
            CCHEX_RET_PICTURE_GET_DATA_BY_EID_TIME_TYPE = 93,
            CCHEX_RET_PICTURE_DEL_DATA_BY_EID_TIME_TYPE = 94,
            CCHEX_RET_LIVE_SEND_SPECIAL_STATUS_TYPE = 95,

            CCHEX_RET_TM_ALL_RECORD_INFO_TYPE = 150,              //CCHEX_RET_TM_RECORD_INFO_STRU
            CCHEX_RET_TM_NEW_RECORD_INFO_TYPE = 151,              //CCHEX_RET_TM_RECORD_INFO_STRU
            CCHEX_RET_TM_LIVE_SEND_RECORD_INFO_TYPE = 152,              //CCHEX_RET_TM_LIVE_SEND_RECORD_INFO_STRU
            CCHEX_RET_TM_UPLOAD_RECORD_INFO_TYPE = 153,              //CCHEX_RET_TM_UPLOAD_RECORD_STRU
            CCHEX_RET_TM_RECORD_BY_EMPLOYEE_TIME_TYPE = 154,              //CCHEX_RET_TM_RECORD_INFO_STRU

            CCHEX_RET_GET_T_RECORD_NUMBER_TYPE = 155,              //CCHEX_RET_GET_T_RECORD_NUMBER_STRU
            CCHEX_RET_GET_T_RECORD_TYPE = 156,              //CCHEX_RET_GET_T_RECORD_STRU
            CCHEX_RET_GET_T_PICTURE_BY_RECORD_ID_TYPE = 157,              //CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU
            CCHEX_RET_DEL_T_PICTURE_BY_RECORD_ID_TYPE = 158,              //CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU

            CCHEX_RET_DOWNLOAD_FACE_PICTURE_MODULE_TYPE = 159,              //CCHEX_DOWNLOAD_FACE_PICTURE_MODULE
            CCHEX_RET_UPLOAD_FACE_PICTURE_MODULE_TYPE = 160,              //CCHEX_UPLOAD_FACE_PICTURE_MODULE
            CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE = 161,          //CCHEX_DOWNLOAD_FACE_PICTURE_MODULE


            CCHEX_RET_CLINECT_CONNECT_FAIL_TYPE     = 200,
            CCHEX_RET_DEV_LOGIN_CHANGE_TYPE         = 201,

            CCHEX_RET_RECORD_INFO_CARD_BYTE7_TYPE = 251,
            
            CCHEX_SAC_DOWNLOAD_EMPLOYEE_TYPE        = 301,
            CCHEX_SAC_UPLOAD_EMPLOYEE_TYPE          = 302,
            CCHEX_SAC_DOWNLOAD_GROUP_TYPE           = 303,
            CCHEX_SAC_UPLOAD_GROUP_TYPE             = 304,
            CCHEX_SAC_DOWNLOAD_EMPLOYEE_WITH_GROUP_TYPE         = 305,
            CCHEX_SAC_UPLOAD_EMPLOYEE_WITH_GROUP_TYPE           = 306,
            CCHEX_SAC_GET_DOOR_INFO_TYPE                        = 307,
            CCHEX_SAC_SET_DOOR_INFO_TYPE                        = 308,
            CCHEX_SAC_DOWNLOAD_DOOR_GROUP_TYPE                  = 309,
            CCHEX_SAC_UPLOAD_DOOR_GROUP_TYPE                    = 310,

            CCHEX_SAC_UPLOAD_DOOR_WITH_DOORGROUP_TYPE           = 311,
            CCHEX_SAC_DOWNLOAD_DOOR_WITH_DOORGROUP_TYPE         = 312,
            CCHEX_SAC_DOWNLOAD_TIME_FRAME_INFO_TYPE             = 313,
            CCHEX_SAC_UPLOAD_TIME_FRAME_INFO_TYPE               = 314,
            CCHEX_SAC_DOWNLOAD_TIME_FRAME_GROUP_TYPE            = 315,
            CCHEX_SAC_UPLOAD_TIME_FRAME_GROUP_TYPE              = 316,
            CCHEX_SAC_DOWNLOAD_TIME_FRAME_WITH_TIME_GROUP_TYPE  = 317,
            CCHEX_SAC_UPLOAD_TIME_FRAME_WITH_TIME_GROUP_TYPE    = 318,
            CCHEX_SAC_DOWNLOAD_ACCESS_CONTROL_GROUP_TYPE        = 319,
            CCHEX_SAC_UPLOAD_ACCESS_CONTROL_GROUP_TYPE          = 320,

            CCHEX_SAC_DOWNLOAD_COMMON_TYPE = 401,
            CCHEX_SAC_UPLOAD_COMMON_TYPE = 402,
            CCHEX_SAC_DELETE_COMMON_TYPE = 403,
            CCHEX_SAC_INIT_COMMON_TYPE = 404,
            CCHEX_SAC_PUSH_COMMON_TYPE = 405,
        };
        public const int FP_LEN = 15360;

        public enum CMD_PRINCIPAL : int
        {
            CMD_Pri_Unknow = 0,  //
            CMD_Pri_UdpDevice = 1,  //Udp设备：搜索，设置        (下载(搜索)/上传(设置))       
            CMD_Pri_AtIniConfig = 2,  //设备配置：下载，上传        (下载/上传(设置))
            CMD_Pri_AtRecord = 3,  //记录                       (下载/上传)
            CMD_Pri_MaType = 4,  //设备类型                    (下载/上传)
            CMD_Pri_MaDateTime = 5,  //设备时间                    (下载/上传)
            CMD_Pri_NetPara = 6,  //网络参数                   (下载/上传)
            CMD_Pri_DeviceURL = 7,  //设备URL                    (下载/上传)
            CMD_Pri_DoorStatus = 8,  //门状态                    (下载/上传)
            CMD_Pri_AtEvent = 9,  //设备事件                   (下载/推送)                    下载上传最大数: 500
            CMD_Pri_StaffInfo = 50,  //员工信息                   (下载/上传/删除/初始化)         下载上传最大数: 500
            CMD_Pri_StaffGroup = 51,  //员工组                     (下载/上传/删除)               下载上传最大数: 50
            CMD_Pri_StaffCombi = 52,  //员工组合(员工+员工组)       (下载/上传/删除)               下载上传最大数: 500
            CMD_Pri_Door = 54,  //门                         (下载/上传/删除)               下载上传最大数: 50
            CMD_Pri_DoorGroup = 55,  //门组                       (下载/上传/删除)               下载上传最大数: 50
            CMD_Pri_DoorCombi = 56,  //门组合(门+门组)             (下载/上传/删除)               下载上传最大数: 500
            CMD_Pri_TimeSpace = 57,  //门禁时段                    (下载/上传/删除)               下载上传最大数: 50
            CMD_Pri_TimeSpaceGroup = 58,  //门禁时段组                  (下载/上传/删除)               下载上传最大数: 500
            CMD_Pri_TimeSpaceCombi = 59,  //门禁时段组合(时段+时段组)    (下载/上传/删除)               下载上传最大数: 500
            CMD_Pri_DoorTimeSpaceCombi = 60,  //门禁时段组合----门          (下载/上传/删除)               下载上传最大数: 50 
            CMD_Pri_FirstCardGroup = 61,  //首卡组                     (下载/上传/删除)               下载上传最大数:  50
            CMD_Pri_FirstCardCombi = 62,  //首卡组合(员工组+首卡组)      (下载/上传/删除)              下载上传最大数:  500
            CMD_Pri_DoorFirstCardCombi = 63,  //首卡组合----门              (下载/上传/删除)              下载上传最大数:  50
            CMD_Pri_MultiCGroup = 64,  //多卡组                     (下载/上传/删除)               下载上传最大数:  50
            CMD_Pri_MultiCGroupIDCount = 65,  //多卡组拼合(5个多卡组及其内员工数)    (下载/上传/删除)       下载上传最大数:   500
            CMD_Pri_MultiCCombi = 66,  //多卡组组合(员工+多卡组)             (下载/上传/删除)       下载上传最大数:   500
            CMD_Pri_DoorMultiCCombi = 67,  //多卡组拼合----门            (下载/上传/删除)               下载上传最大数:   50
            CMD_Pri_Holidays = 68,  //节假日设置　　　            (下载/上传/删除)               下载上传最大数:    50
            CMD_Pri_AntiBack = 69,  //反潜信息记录表              (下载/上传/删除)               下载上传最大数:    500
        }

            [StructLayout(LayoutKind.Sequential, Size = 27, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_NETCFG_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] IpAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] IpMask;   //子网掩码
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MacAddr;  //MAC地址
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] GwAddr;   //网关地址
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] ServAddr; //服务器ip
            public byte RemoteEnable;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Port;     //端口
            public byte Mode;       //方式
            public byte DhcpEnable;
        } //27 bytes



        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_COMMON_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
        };

        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_REBOOT_STRU
        {
            public uint MachineId;
            public int Result;
        }

        //数组网络
        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_SETNETCFG_STRU
        {
            public uint MachineId;
            public int Result;
        }

        //更新回复
        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_UPLOADFILE_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public uint TotalBytes;
            public uint SendBytes;
        }

        //网络配置回复
        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GETNETCFG_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public CCHEX_NETCFG_INFO_STRU Cfg;
        }

        [StructLayout(LayoutKind.Sequential, Size = 14, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UPLOAD_RECORD_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] date;          //日期时间
            public byte back_id;           //备份号
            public byte record_type;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] work_type;      //工种        (ONLY use 3bytes )
        }
        [StructLayout(LayoutKind.Sequential, Size = 37, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UPLOAD_RECORD_INFO_STRU_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] date;          //日期时间
            public byte back_id;           //备份号
            public byte record_type;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] work_type;      //工种        (ONLY use 3bytes )
        }

        [StructLayout(LayoutKind.Sequential, Size = 14+4+4, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU
        {
            public uint MachineId;         //机器号
            int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
        }
        [StructLayout(LayoutKind.Sequential, Size = 37+4+4, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU_VER_4_NEWID
        {
            public uint MachineId;         //机器号
            int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
        }

        [StructLayout(LayoutKind.Sequential, Size = 28, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_RECORD_INFO_STRU
        {
            public uint MachineId;         //机器号
            public byte NewRecordFlag;    //是否是新记录
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
            public byte Rsv;

            public uint CurIdx;             //add  VER 22
            public uint TotalCnt;           //add  VER 22
        }
        [StructLayout(LayoutKind.Sequential, Size = 30, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_RECORD_INFO_STRU_CARD_ID_LEN_7
        {
            public uint MachineId;         //机器号
            public byte NewRecordFlag;    //是否是新记录
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] CardId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
            public byte Rsv;

            public uint CurIdx;             //add  VER 22
            public uint TotalCnt;           //add  VER 22
        }
        [StructLayout(LayoutKind.Sequential, Size = 51, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID
        {
            public uint MachineId;         //机器号
            public byte NewRecordFlag;    //是否是新记录
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
            public byte Rsv;

            public uint CurIdx;             //add  VER 22
            public uint TotalCnt;           //add  VER 22
        }


        //device login-logout struct
        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEV_LOGIN_STRU
        {
            public int DevIdx;
            public uint MachineId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Addr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Version;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevType;
            public uint DevTypeFlag;
        }

        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEV_LOGOUT_STRU
        {
            public int DevIdx;
            public uint MachineId;
            public uint Live;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Addr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Version;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevType;
        }



        [StructLayout(LayoutKind.Sequential, Size = 11, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSGHEAD_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            public byte StartYear;
            public byte StartMonth;
            public byte StartDay;

            public byte EndYear;
            public byte EndMonth;
            public byte EndDay;
        }
        [StructLayout(LayoutKind.Sequential, Size = 34, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            public byte StartYear;
            public byte StartMonth;
            public byte StartDay;

            public byte EndYear;
            public byte EndMonth;
            public byte EndDay;
        }

        [StructLayout(LayoutKind.Sequential, Size = 17, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSGHEADUNICODE_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            public byte StartYear;
            public byte StartMonth;
            public byte StartDay;
            public byte StartHour;
            public byte StartMin;
            public byte StartSec;

            public byte EndYear;
            public byte EndMonth;
            public byte EndDay;
            public byte EndHour;
            public byte EndMin;
            public byte EndSec;
        }
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            public byte StartYear;
            public byte StartMonth;
            public byte StartDay;
            public byte StartHour;
            public byte StartMin;
            public byte StartSec;

            public byte EndYear;
            public byte EndMonth;
            public byte EndDay;
            public byte EndHour;
            public byte EndMin;
            public byte EndSec;
        }




        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_MSGGETBYIDX_UNICODE_STRU
        {
            public uint MachineId;
            public int Result;
            public int Len;
        }

        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_MSGADDNEW_UNICODE_STRU
        {
            public uint MachineId;
            public int Result;
            public int Len;
        }

        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU
        {
            public uint MachineId;
            public int Result;
            public int Len;
        }



        [StructLayout(LayoutKind.Sequential, Size = 9, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_MSGDELBYIDX_UNICODE_STRU
        {
            public uint MachineId;
            public int Result;
            public byte Idx;
        }

        [StructLayout(LayoutKind.Sequential, Size = 28, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEV_STATUS_STRU
        {
            public uint MachineId;
            public uint EmployeeNum;
            public uint FingerPrtNum;
            public uint PasswdNum;
            public uint CardNum;
            public uint TotalRecNum;
            public uint NewRecNum;
        }

        public const int SN_LEN = 16;
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_SN_STRU
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SN_LEN)]
            public byte[] sn;
        }

        // basic config info
        [StructLayout(LayoutKind.Sequential, Size = 20, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_BASIC_CFG_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] software_version;
            public uint password;
            public byte delay_for_sleep;
            public byte volume;
            public byte language;
            public byte date_format;
            public byte time_format;
            public byte machine_status;
            public byte modify_language;
            public byte cmd_version;
        } //20 bytes

        [StructLayout(LayoutKind.Sequential, Size = 13, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_BASIC_CFG_INFO_STRU
        {
            public uint password;
            public byte pwd_len;
            public byte delay_for_sleep;
            public byte volume;
            public byte language;
            public byte date_format;
            public byte time_format;
            public byte machine_status;
            public byte modify_language;
            public byte reserved;
        } //13 bytes

        [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_BASIC_CFG_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public CCHEX_GET_BASIC_CFG_INFO_STRU Cfg;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_EMPLOYEE_INFO_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] padding;  // 5 bytes,12 digitals
        }
        [StructLayout(LayoutKind.Sequential, Size = 36, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;  // 
        }
        // list,modify and delete person
        [StructLayout(LayoutKind.Sequential, Size = 294, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_PERSON_INFO_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            public byte password_len;
            public byte max_password;  // now only 6 digitals, do not modify
            public int password;       // do not exceed max_password digitals
            public byte max_card_id;   // 6 for 3 bytes,10 for 4 bytes, do not modify
            public uint card_id;       // do not exceed max_card_id digitals
            public byte max_EmployeeName;  // may be 10 or 20 or 64, do not modify
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] EmployeeName;// do not exceed max_EmployeeName digitals

            public byte DepartmentId;
            public byte GroupId;
            public byte Mode;
            public uint Fp_Status;    // 0~9:fp; 10:face; 11:iris1; 12:iris2
            public byte Rserved1;      // for 22
            public byte Rserved2;      // for 72 and 22
            public byte Special;

            // DR info
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[] EmployeeName2;// do not exceed max_EmployeeName digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 13)]
            public byte[] RFC;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] CURP;
        }

        [StructLayout(LayoutKind.Sequential, Size = 60, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            public byte password_len;
            public byte max_password;  // now only 6 digitals, do not modify
            public int password;       // do not exceed max_password digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] card_id;       // do not exceed max_card_id digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] EmployeeName;// do not exceed max_EmployeeName digitals

            public byte DepartmentId;
            public byte GroupId;
            public byte Mode;
            public uint Fp_Status;    // 0~9:fp; 10:face; 11:iris1; 12:iris2
            public byte Rserved1;      // for 22
            public byte Rserved2;      // for 72 and 22
            public byte Special;
        }
        [StructLayout(LayoutKind.Sequential, Size = 43, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Passwd;  // 5 bytes,12 digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] CardId;       // do not exceed max_card_id digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] EmployeeName;// do not exceed max_EmployeeName digitals

            public byte DepartmentId;
            public byte GroupId;
            public byte Mode;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FpStatus;// do not exceed max_EmployeeName digitals
            public byte Rserved1;      // for 22
            public byte Rserved2;      // for 72 and 22
            public byte Special;
        }

        [StructLayout(LayoutKind.Sequential, Size = 302, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2
        {
            uint MachineId;
            int CurIdx;
            int TotalCnt;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            public byte password_len;
            public byte max_password;  // now only 6 digitals, do not modify
            public int password;       // do not exceed max_password digitals
            public byte max_card_id;   // 6 for 3 bytes,10 for 4 bytes, do not modify
            public uint card_id;       // do not exceed max_card_id digitals
            public byte max_EmployeeName;  // may be 10 or 20 or 64, do not modify
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] EmployeeName;// do not exceed max_EmployeeName digitals

            public byte DepartmentId;
            public byte GroupId;
            public byte Mode;
            public uint Fp_Status;    // 0~9:fp; 10:face; 11:iris1; 12:iris2
            public byte Rserved1;      // for 22
            public byte Rserved2;      // for 72 and 22
            public byte Special;

            // DR info
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[] EmployeeName2;// do not exceed max_EmployeeName digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 13)]
            public byte[] RFC;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] CURP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] start_date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] end_date;

        }
        [StructLayout(LayoutKind.Sequential, Size = 325, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4
        {
            public  uint MachineId;
            public int CurIdx;
            public int TotalCnt;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            public byte password_len;
            public byte max_password;  // now only 6 digitals, do not modify
            public int password;       // do not exceed max_password digitals
            public byte max_card_id;   // 6 for 3 bytes,10 for 4 bytes, do not modify
            public uint card_id;       // do not exceed max_card_id digitals
            public byte max_EmployeeName;  // may be 10 or 20 or 64, do not modify
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] EmployeeName;// do not exceed max_EmployeeName digitals

            public byte DepartmentId;
            public byte GroupId;
            public byte Mode;
            public uint Fp_Status;    // 0~9:fp; 10:face; 11:iris1; 12:iris2
            public byte Rserved1;      // for 22
            public byte Rserved2;      // for 72 and 22
            public byte Special;

            // DR info
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[] EmployeeName2;// do not exceed max_EmployeeName digitals
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 13)]
            public byte[] RFC;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] CURP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] start_date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] end_date;
        }
        /*[StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_PERSON_INFO_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public CCHEX_PERSON_INFO_STRU person;
        }*/

        [StructLayout(LayoutKind.Sequential, Size = 6, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_DEL_PERSON_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes,12 digitals
            public byte operation;
        }
        [StructLayout(LayoutKind.Sequential, Size = 29, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;  //
            public byte operation;
        }

        //获取单个员工资料
        [StructLayout(LayoutKind.Sequential, Size = 5, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_ONE_EMPLOYEE_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;  // 5 bytes
        }
        [StructLayout(LayoutKind.Sequential, Size = 28, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_ONE_EMPLOYEE_INFO_STRU_ID_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;  //
        }

        // download finger print
        [StructLayout(LayoutKind.Sequential, Size = FP_LEN + 18, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DLFINGERPRT_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            public byte FpIdx;
            public uint fp_len;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = FP_LEN)]
            public byte[] Data;
        }
        // download finger print
        [StructLayout(LayoutKind.Sequential, Size = FP_LEN + 41, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            public byte FpIdx;
            public uint fp_len;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = FP_LEN)]
            public byte[] Data;
        }


        // delete record or new flag
        [StructLayout(LayoutKind.Sequential, Size = 5, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_DEL_RECORD_INFO_STRU
        {
            public byte del_type;
            public uint del_count;
        }

        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_RECORD_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public uint deleted_count;
        }

        //get time~~~~~~~~~~~~~~~~~~~~~
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSG_GETTIME_STRU
        {
            public uint Year;
            public uint Month;
            public uint Day;
            public uint Hour;
            public uint Min;
            public uint Sec;
        } 
        [StructLayout(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MSG_GETTIME_STRU_EXT_INF
        {
            public uint MachineId;
            public int  Result; //0 ok, -1 err
            public CCHEX_MSG_GETTIME_STRU config;
        } 

        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF
        {
            public byte compare_level;
            public byte wiegand_range;
            public byte wiegand_type;
            public byte work_code;
            public byte real_time_send;
            public byte auto_update;
            public byte bell_lock;
            public byte lock_delay;
            public uint record_over_alarm;
            public byte re_attendance_delay;
            public byte door_sensor_alarm;
            public byte bell_delay;
            public byte correct_time;
        }
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF
        {
            public uint MachineId;
            public int  Result; //0 ok, -1 err
            public CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF config;
        }
//Period of time
        [StructLayout(LayoutKind.Sequential, Size = 4, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF
        {
            public byte StartHour;
            public byte StartMin;
            public byte EndHour;
            public byte EndMin;
        }

        [StructLayout(LayoutKind.Sequential, Size = 36, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_PERIOD_TIME_STRU_EXT_INF
        {
            public uint MachineId;
            public int  Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[] day_week;
        }

        [StructLayout(LayoutKind.Sequential, Size = 29, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_PERIOD_TIME_STRU_EXT_INF
        {
            public byte SerialNumbe;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[] day_week;
        }

//Team Info
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_TEAM_INFO_STRU_EXT_INF
        {
            public uint     MachineId;
            public int      Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   PeriodTimeNumber;   
        }

        [StructLayout(LayoutKind.Sequential, Size = 5, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_TEAM_INFO_STRU_EXT_INF
        {
            public byte     TeamNumbe;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   PeriodTimeNumber;
        }
        //bell  Info
        [StructLayout(LayoutKind.Sequential, Size = 3, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_BELL_TIME_POINT
        {
            public byte hour;
            public byte minute;
            public byte flag_week;
        }

        [StructLayout(LayoutKind.Sequential, Size = 100, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_BELL_INFO_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 30)]
            public CCHEX_RET_GET_BELL_TIME_POINT[] time_point;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] padding;
        }
        //Add
        [StructLayout(LayoutKind.Sequential, Size = 6, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[]   EmployeeId;  // 5 bytes
            public byte     BackupNum;
        }
        [StructLayout(LayoutKind.Sequential, Size = 29, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF_ID_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;  // 5 bytes
            public byte BackupNum;
        }


        //forced unlock
        [StructLayout(LayoutKind.Sequential, Size = 6, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_FORCED_UNLOCK_STRU_EXT_INF
        {
            public byte     LockCmd;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[]   EmployeeId;
        }

//UDP  search dev       
        [StructLayout(LayoutKind.Sequential, Size = 25, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_DEV_NET_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   IpAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   IpMask;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   GwAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[]   MacAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   ServAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[]   Port;
            public byte     NetMode;
        } //25 bytes

        [StructLayout(LayoutKind.Sequential, Size = 63, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[]   DevType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[]   DevSerialNum;
            public CCHEX_DEV_NET_INFO_STRU DevNetInfo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[]   Version;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Reserved;
        } //  63   ::  0:Dev without DNS;

        [StructLayout(LayoutKind.Sequential, Size = 167, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_WITH_DNS_STRU
        {
            public CCHEX_UDP_SEARCH_STRU BasicSearch;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Dns;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[]   Url;
        }//  167  ::  1:Dev has DNS;

        [StructLayout(LayoutKind.Sequential, Size = 28, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_CARD_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[]   CardName;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   IpAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   IpMask;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   GwAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[]   MacAddr;
        } //28

        [StructLayout(LayoutKind.Sequential, Size = 102, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_TWO_CARD_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[]   DevType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[]   DevSerialNum;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   ServAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[]   Port;
            public byte     NetMode;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[]   Version;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Reserved;
            public byte     CardNumber;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public CCHEX_UDP_SEARCH_CARD_STRU[] CardInfo;
        }//46+28*2 = 102    ::   2:Dev has two NetCard

        [StructLayout(LayoutKind.Sequential, Size = 180, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_STRU_EXT_INF
        {
            
                                    //0:CCHEX_UDP_SEARCH_STRU;1:CCHEX_UDP_SEARCH_WITH_DNS_STRU;2:CCHEX_UDP_SEARCH_TWO_CARD_STRU
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 167)]
            public byte[]   Data;
            public byte     Padding;
            public int Result; //0: ok -1: fail
            public uint MachineId;
            
            public int DevHardwareType;    //0:Dev without DNS;1:Dev has DNS;2:Dev has two NetCard
        }//8+167+1

        [StructLayout(LayoutKind.Sequential, Size = 9004, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SEARCH_ALL_STRU_EXT_INF
        {
            public int      DevNum;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 50)]
            public  CCHEX_UDP_SEARCH_STRU_EXT_INF[] dev_net_info;
        }//4+DevNum*sizeof(CCHEX_UDP_SEARCH_STRU_EXT_INF)

//UDP  set dev config
        [StructLayout(LayoutKind.Sequential, Size = 168, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UDP_SET_DEV_CONFIG_STRU_EXT_INF
        {
            public CCHEX_DEV_NET_INFO_STRU DevNetInfo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[]   Padding;
            public uint     NewMachineId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Reserved;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[]   DevUserName;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[]   DevPassWord;
            public int      DevHardwareType;//0:Dev without DNS;1:Dev has DNS;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Dns;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[]   Url;
        }//

        //3.0
        [StructLayout(LayoutKind.Sequential, Size = 168, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_USER_ATTENDANCE_STATUS_STRU
        {
            public uint     fp_len;                        //data_info :: ANSI VERSION   fp_len = 80  UNICODE VERSION   fp_len = 160
            public byte     atten_status_number;          //Param:8 grounp
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[]   data_info;               //Param:8 grounp  ANSI VERSION: unsigned char [8][10]  UNICODE VERSION: unsigned char[8][20] 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[]   padding;
        }
        [StructLayout(LayoutKind.Sequential, Size = 7, CharSet = CharSet.Ansi), Serializable]
        public struct GET_DST_PARAM_TIME
        {
            public byte     month;
            public byte     day;
            public byte     week_num;       //周次定义如下：0x01-0x04：前1-4周 0x81-0x82：后1-2周
            public byte     flag_week;      //星期标志flag_week 0-6:(用二进制0000000分别表示：六五四三二一日)
            public byte     hour;
            public byte     minute;
            public byte     sec;
        }

        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_DST_PARAM_STRU
        {
            public byte     enabled;          //0-不启用      1-启用；
            public byte     date_week_type;   //1-日期格式	2-星期格式；
            public GET_DST_PARAM_TIME start_time;
            public GET_DST_PARAM_TIME special_time;
        }

        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_DST_PARAM_STRU
        {
            public uint     MachineId;
            public int      Result;
            public CCHEX_SET_DST_PARAM_STRU param;
        }

        [StructLayout(LayoutKind.Sequential, Size = 320, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_DEV_EXT_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 50)]
            public byte[]   manufacturer_name;         //厂商名称
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[]   manufacturer_addr;        //厂商地址
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[]   duty_paragraph;            //税号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 155)]
            public byte[]   reserved;                 //预留
        }

        [StructLayout(LayoutKind.Sequential, Size = 328, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_DEV_EXT_INFO_STRU
        {
            public uint     MachineId;
            public int      Result;
            public CCHEX_SET_DEV_EXT_INFO_STRU param;
        }

        [StructLayout(LayoutKind.Sequential, Size = 15, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_BASIC_CFG_INFO3_STRU
        {
            public byte     wiegand_type;         //韦根读取方式
            public byte     online_mode;
            public byte     collect_level;
            public byte     pwd_status;           //连接密码状态  =0 网络连接时不需要验证通讯密码 =1网络连接时需要先发送0x04命令 验证通讯密码
            public byte     sensor_status;           //=0 不主动汇报门磁状态  =1主动汇报门磁状态（设备主动发送0x2F命令的应答包)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[]   reserved;
            public byte     independent_time;
            public byte     m5_t5_status;         //= 0	禁用 = 1	启用，本机状态为出=2	启用，本机状态为入 =4	禁用，本机状态为出 =5	禁用，本机状态为入
        }

        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_BASIC_CFG_INFO3_STRU
        {
            public uint     MachineId;
            public int      Result;
            public CCHEX_SET_BASIC_CFG_INFO3_STRU param;
            public byte     padding;
        }

        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_CONNECTION_AUTHENTICATION_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[]   username;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[]   password;
        }

        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_RECORD_NUMBER_STRU
        {
            public uint     MachineId;
            public int      Result;
            public int      record_num;
        }

        [StructLayout(LayoutKind.Sequential, Size = 13, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_RECORD_INFO_BY_TIME
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[]   EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   start_date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   end_date;
        }
        [StructLayout(LayoutKind.Sequential, Size = 36, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] start_date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] end_date;
        }

        [StructLayout(LayoutKind.Sequential, Size = 24+8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU
        {
            public uint     MachineId;
            public int      Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[]   EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   date;
            public byte     back_id;                      //备份号
            public byte     record_type;                  //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[]   work_type; //工种        (ONLY use 3bytes )
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[]   padding;
            public int CurIdx;
            public int TotalCnt;
        }
        [StructLayout(LayoutKind.Sequential, Size = 47+8, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] date;
            public byte back_id;                      //备份号
            public byte record_type;                  //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] work_type; //工种        (ONLY use 3bytes )
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] padding;
            public int CurIdx;
            public int TotalCnt;
        }

        [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_BASIC_CFG_INFO5_STRU
        {
            public byte     fail_alarm_time;
            public byte     tamper_alarm;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 94)]
            public byte[]   reserved;
        }
        [StructLayout(LayoutKind.Sequential, Size = 104, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_BASIC_CFG_INFO5_STRU
        {
            public uint     MachineId;
            public int      Result;
            public CCHEX_SET_BASIC_CFG_INFO5_STRU param;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_CARD_ID_STRUF
        {
            public uint     MachineId;
            public int      Result;
            public uint     card_id;
        }

        [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_DEV_CURRENT_STATUS_STRU
        {
            public byte     alarm_stop;
            public byte     door_status;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 94)]
            public byte[]   reserved;
        }

        [StructLayout(LayoutKind.Sequential, Size = 112, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_GET_URL_STRU
        {
            public uint     MachineId;
            public int      Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[]   Dns;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[]   Url;
        }


        [StructLayout(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_STATUS_SWITCH_STRU
        {
            public byte     group_id;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF [] day_week;
            public byte     status_id;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[]   padding;
        }
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_STATUS_SWITCH_STRU
        {
            public uint     MachineId;
            public int      Result;
            public byte     group_id;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[] day_week;
            public byte     status_id;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[]   padding;
        }


        [StructLayout(LayoutKind.Sequential, Size = 4, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_ONE_TIMER_STATUS
        {
            public byte     StartHour;
            public byte     StartMin;
            public byte     EndHour;
            public byte     EndMin;
            public byte     status_id;
        }
        [StructLayout(LayoutKind.Sequential, Size = 44, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_SET_STATUS_SWITCH_STRU_EXT
        {
            public byte     flag_week;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public CCHEX_ONE_TIMER_STATUS[] one_time;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[]   padding;
        }
        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT
        {
            public uint     MachineId;
            public int      Result;
            public byte     flag_week;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public CCHEX_ONE_TIMER_STATUS[] one_time;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[]   padding;
        }

        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_UPDATEFILE_STATUS
        {
            public uint     MachineId;
            public int      Result;
            public int      verify_status;
            public int      verify_ret;
        }

        //add 3.0
        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_SPECIAL_STATUS_STRU
        {
            public uint MachineId;
            public int  Result;
            public byte status;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] reserved;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_MACHINE_ID_STRU
        {
            public uint MachineId;
            public int Result;
            public uint cur_machineid;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_SET_MACHINE_ID_STRU
        {
            public uint MachineId;
            public int Result;
            public uint cur_machineid;
            public uint old_machineid;
        }

        [StructLayout(LayoutKind.Sequential, Size = 20, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_MACHINE_TYPE_STRU
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DevType;
            public int DevTypeFlag;
        }
        /*
         * start_date[4];                //相距2000年后的秒数2000.1.2
         * end_date[4];                  //相距2000年后的秒数2000.1.2
         * CmdType;                      扩展命令字：　0x00 获取当前时间段内总条数  0x01 下载记录 0x02  删除当前时间段内所有记录 
                                                0x03 设置实时上报日志记录   0x04 获取实时上报记录状态  
         *AutoFlag;                     //0:关闭实时上报日志记录 1:开启实时上报日志记录
         */
        [StructLayout(LayoutKind.Sequential, Size = 10, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_MANAGE_LOG_RECORD
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] start_date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] end_date;
            public byte CmdType;
            public byte AutoFlag;
        }
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_MANAGE_LOG_RECORD
        {
            public uint MachineId;
            public uint CmdType;
            public int Result;
            public uint IsAuto;
            public uint TotalNum;
            public uint CurNum;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] LogType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] LogLen;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] padding;
        }

        /********************************************
         * SAC :standard access control
         ***********************************************/
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_EMPLOYEE_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Password;                      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Card;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] EmployeeName;
            public byte CardType;
            public byte GroupId;
            public byte AttendanceMode;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FpStatus;
            public byte PwdH8bit;
            public byte HolidayGroup;
            public byte Special;
        }

        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_DOWNLOAD_EMPLOYEE_INFO_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;
            public SAC_EMPLOYEE_INFO_STRU employee;
        }
        /*
组信息:
GroupId     :组编号
GroupName   :组名
*/
        [StructLayout(LayoutKind.Sequential, Size = 101, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_GROUP_INFO_STRU
        {
            public byte GroupId;      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] GroupName;                      // 
            public byte uGroupType;
        }

        [StructLayout(LayoutKind.Sequential, Size = 113, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_GROUP_INFO_STRU
        {
            public uint MachineId;
            public int  CurIdx;
            public int  TotalCnt;
            public SAC_GROUP_INFO_STRU GroupData;
        }
        [StructLayout(LayoutKind.Sequential, Size = 113, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_UPLOAD_GROUP_INFO_STRU
        {
            public uint MachineId;
            public int  Result;
            public uint GroupFlag;
        }
        /*
 员工的组信息:
 EmployeeId  :用户号
 GroupId     :组编号
 */
        [StructLayout(LayoutKind.Sequential, Size = 6, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_EMPLOYEE_WITH_GROUP_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;                      // 
            public byte GroupId;      // 
        }
        /*
        下载组信息:
        MachineId   :机器号
        CurIdx      :当前序号
        TotalCnt    :下载总数
        Data        :员工的组信息 SAC_EMPLOYEE_WITH_GROUP_INFO_STRU
        */
        [StructLayout(LayoutKind.Sequential, Size = 113, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;
            public SAC_EMPLOYEE_WITH_GROUP_INFO_STRU Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 113, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_UPLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
        }
        /*
        门信息:
        DoorId          :门号
        DoorName        :门名
        DevName         :设备名(门名称默认值: “设备名-门编号”   (UNICODE编码))
        Anti_subType    :反潜类型 0：未开启反潜 1：单门反潜 2：双门反潜
        InterlockFlag   :是否互锁   0: 未开启     1：开启
        InterlockDoorId :互锁门号   (互锁门号不能与当前门相同门号)
        DoorStatus      :门状态   0：重置  1：普通  2：多卡开门   3：首卡常开 4：常关 5：常开 6：错误
        */
        [StructLayout(LayoutKind.Sequential, Size = 145, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_SET_DOOR_INFO_STRU
        {
            public byte DoorId;      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] DoorName;                      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] DevName;
            public byte Anti_subType;      // 
            public byte InterlockFlag;      // 
            public byte InterlockDoorId;      // 
            public byte DoorStatus;      // 
        }
        [StructLayout(LayoutKind.Sequential, Size = 145+8, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_GET_DOOR_INFO_STRU
        {
            public uint MachineId;
            public int Result;
            public SAC_SET_DOOR_INFO_STRU Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_SET_DOOR_INFO_STRU
        {
            public uint MachineId;
            public int Result; //0 ok, -1 err
            public uint DoorId;
        }

        /*
门组合信息:门 与 门组 关联信息
CombinationId       :组合号
DoorId              :门号
DoorGroupId         :门组号
*/
        [StructLayout(LayoutKind.Sequential, Size = 3, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_DOOR_WITH_DOORGROUP_INFO_STRU
        {
            public byte CombinationId;      // 
            public byte DoorId;      // 
            public byte DoorGroupId;      // 
        }
        [StructLayout(LayoutKind.Sequential, Size = 3 + 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_DOOR_WITH_DOORGROUP_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;
            public SAC_DOOR_WITH_DOORGROUP_INFO_STRU Data;
        }
        /*
上传 下载时段信息:
MachineId   :机器号
Result      :执行结果 ==0:成功
TimeFrameNum:时间段序号
date        :0: 星期一子时段,0: 星期二子时段,0: 星期三子时段,0: 星期四子时段,0: 星期五子时段,
             0: 星期六子时段,0: 星期日子时段,0: 节假日一时段,0: 节假日二时段,0: 节假日三时段
        unsigned char StartHour :开始小时
        unsigned char StartMin  :开始分钟
        unsigned char EndHour   :开始小时
        unsigned char EndMin    :开始分钟
*/
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_UPLOAD_TIME_FRAME_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
            public CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[] date;
        }
        [StructLayout(LayoutKind.Sequential, Size = 40+12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_TIME_FRAME_INFO_STRU
        {
            public uint MachineId;
            public int Result;
            public uint TimeFrameNum;
            public SAC_UPLOAD_TIME_FRAME_INFO_STRU Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_UPLOAD_TIME_FRAME_INFO_STRU
        {
            public uint MachineId;
            public int Result;
            public uint TimeFrameNum;
        }
        /*
时间段组合信息:时间段 与 时间段组 关联信息
CombinationId       :组合号
TimeFrameId         :时段号
TimeFrameGroupId    :时段组号
*/
        [StructLayout(LayoutKind.Sequential, Size = 3, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_TimeFrame_WITH_TimeGROUP_INFO_STRU
        {
            public byte CombinationId;      // 
            public byte TimeFrameId;      // 
            public byte TimeFrameGroupId;      // 
        }
        [StructLayout(LayoutKind.Sequential, Size = 3 + 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_TimeFrame_WITH_TimeGROUP_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;
            public SAC_TimeFrame_WITH_TimeGROUP_INFO_STRU Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_UPLOAD_TimeFrame_WITH_TimeGROUP_STRU
        {
            public uint MachineId;
            public int Result;
            public uint GroupFlag;
        }
        /*
门禁时段组信息:
SAC_GroupId         :组编号
GroupName           :组名
EmployeeGroupId     :用户组号
DoorGroupId         :门组号
TimeGroupId         :时段组号
*/
        [StructLayout(LayoutKind.Sequential, Size = 104, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_ACCESS_CONTROL_GROUP_INFO_STRU
        {
            public byte SAC_GroupId;      // 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] GroupName;
            public byte EmployeeGroupId;      // 
            public byte DoorGroupId;      // 
            public byte TimeGroupId;      // 
        }
        [StructLayout(LayoutKind.Sequential, Size = 104 + 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_RET_DOWNLOAD_CONTROL_GROUP_INFO_STRU
        {
            public uint MachineId;
            public int CurIdx;
            public int TotalCnt;
            public SAC_ACCESS_CONTROL_GROUP_INFO_STRU Data;
        }
        /***********************************
         * 功能 : 获取照片文件总数,获取照片文件头信息,获取指定照片文件,删除指定照片文件
        ************************************/
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_NUMBER_STRU
        {
            public uint MachineId;
            public int Result;
            public uint PictureTotal;
        }
        [StructLayout(LayoutKind.Sequential, Size = 25, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU
        {
            public uint MachineId;
            public int Result;
            public int CurIdx;
            public int TotalCnt;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 48, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID
        {
            public uint MachineId;
            public int Result;
            public int CurIdx;
            public int TotalCnt;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 9, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_PICTURE_BY_EID_AND_TIME
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME
        {
            public uint MachineId;
            public int Result;
            public uint DataLen;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] padding;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] PictureData;
        }
        [StructLayout(LayoutKind.Sequential, Size = 44, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID
        {
            public uint MachineId;
            public int Result;
            public uint DataLen;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] PictureData;
        }


        /*******************************************
         * 图片人脸模板
        *******************************************/
        [StructLayout(LayoutKind.Sequential, Size = 20, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_DOWNLOAD_FACE_PICTURE_MODULE
        {
            public uint MachineId;
            public int Result;
            
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;                                       //员工号
            public byte Backup;                                             //备份号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] padding;                                          //填充(格式对齐)
            public uint DataLen;                                            //图片人脸模块数据长度
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] PictureData;                                        //图片人脸模块数据
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_UPLOAD_FACE_PICTURE_MODULE
        {
            public uint MachineId;
            public int Result;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;                                       //员工号
            public byte Backup;                                             //备份号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] padding;                                          //填充(格式对齐)
        }


        [StructLayout(LayoutKind.Sequential, Size = 17, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] EmployeeId;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DateTime;
        }
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_DOWNLOAD_COMMON_RESULT
        {
            public uint MachineId;
            public uint CmdPrincipa;
            public int Result;
            public uint SeqNum;
            public uint MaxCount;
            public uint DataCurCount;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_UPLOAD_COMMON_RESULT
        {
            public uint MachineId;
            public uint CmdPrincipa;
            public int Result;
            public uint DataCurCount;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_DELETE_COMMON_RESULT
        {
            public uint MachineId;
            public uint CmdPrincipa;
            public int Result;
            public uint DataCurCount;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] Data;
        }
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_INIT_COMMON_RESULT
        {
            public uint MachineId;
            public uint CmdPrincipa;
            public int Result;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_PUSH_COMMON_RESULT
        {
            public uint MachineId;
            public uint CmdPrincipa;
            public int Result;
            public uint DataCurCount;
        }
        [StructLayout(LayoutKind.Sequential, Size = 52, CharSet = CharSet.Ansi), Serializable]
        public struct SAC_DATA_Device_Event
        {

            public byte uEventType;//1
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] dwTime;//4
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] uEmID;//5
            public byte uWiegandNo;//1
            public byte uDoorNo;//1
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] szMsg;
    }

        [StructLayout(LayoutKind.Sequential, Size = 34, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_TM_RECORD_INFO_STRU
        {
            public uint MachineId;         //机器号
            int Result; //0 ok, -1 err
            public byte NewRecordFlag;    //是否是新记录
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;          //日期时间
            public byte BackId;           //备份号
            public byte RecordType;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] WorkType;      //工种        (ONLY use 3bytes )
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Temperature;      //温度整数        (ONLY use 3bytes )
            public byte IsMask;
            public byte OpenType;

            public uint CurIdx;             //add  VER 22
            public uint TotalCnt;           //add  VER 22
        }
        [StructLayout(LayoutKind.Sequential, Size = 18, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_TM_UPLOAD_RECORD_INFO_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] EmployeeId;    //
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] date;          //日期时间
            public byte back_id;           //备份号
            public byte record_type;       //记录类型
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] work_type;      //工种        (ONLY use 3bytes )
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Temperature;      //温度整数        (ONLY use 3bytes )
            public byte IsMask;
            public byte OpenType;
        }
        [StructLayout(LayoutKind.Sequential, Size = 9, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_PICTURE_BY_RECORD_ID_STRU
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] RecoradId;    //
            public byte TemperatureType;
        }
        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_T_RECORD_NUMBER_STRU
        {
            public uint MachineId;         //机器号
            public int Result; //0 ok, -1 err
            public int RecordNum;          //记录数量
            public int RecordTpye;         //测温类型
        }
        [StructLayout(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_T_RECORD_STRU
        {
            public uint MachineId;         //机器号
            public int Result; //0 ok, -1 err
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] RecoradId;          
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Date;     
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Temperature;      
            public byte TemperatureType;
            public byte MaskType;
            public int CurIdx;
            public int TotalCnt;        
        }
        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] RecoradId;
            public byte TemperatureType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Padding;
            public uint DataLen;
            //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 0)]
            //public byte[] PictureData;
        }
        [StructLayout(LayoutKind.Sequential, Size = 17, CharSet = CharSet.Ansi), Serializable]
        public struct CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU
        {
            public uint MachineId;
            public int Result;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] RecoradId;
            public byte TemperatureType;
        }



        //function
        [DllImport("tc-b_new_sdk.dll")]
        public static extern uint CChex_Version();

        [DllImport("tc-b_new_sdk.dll")]
        public static extern void CChex_Init();

        /*****************************************************************************
            return AvzHandle
        *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll")]
        public static extern IntPtr CChex_Start();
        /*****************************************************************************
        return AvzHandle
        IsCloseServer:   0 : not close  ,ServerPort 0:random   other:other
        *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CChex_Start_With_Param(uint IsCloseServer, uint ServerPort);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_Set_Service_Port(ushort Port);

        [DllImport("tc-b_new_sdk.dll")]
        public static extern int CChex_Set_Service_Disenable();

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_Get_Service_Port(IntPtr CchexHandle);


        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CChex_Stop(IntPtr CchexHandle);

        //网络
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetNetConfig(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetNetConfig(IntPtr CchexHandle, int DevIdx, ref CCHEX_NETCFG_INFO_STRU Config);//CCHEX_NETCFG_INFO_STRU *

        //消息
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_MsgGetByIdx(IntPtr CchexHandle, int DevIdx, byte Idx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_MsgDelByIdx(IntPtr CchexHandle, int DevIdx, byte Idx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_MsgAddNew(IntPtr CchexHandle, int DevIdx, byte[] Data, int Len);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_MsgGetAllHead(IntPtr CchexHandle, int DevIdx);

        /*****************************************************************************
            return 
                >0  ok, return length
                =0, no result.
                <0  return (0 - need len)
        *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_Update(IntPtr CchexHandle, int[] DevIdx, int[] Type, IntPtr Buff, int Len);

        /*****************************************************************************

        *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_RebootDevice(IntPtr CchexHandle, int DevIdx);

        /*****************************************************************************

        *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetTime(IntPtr CchexHandle, int DevIdx, int Year, int Month, int Day, int Hour, int Min, int Sec);

        //FileType 0: firmware 1:pic 2: audio 3: language file 
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadFile(IntPtr CchexHandle, int DevIdx, byte FileType, byte[] FileName, byte[] Buff, int Len);

        // serial number
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetSNConfig(IntPtr CchexHandle, int DevIdx);

        // download all record
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadAllRecords(IntPtr CchexHandle, int DevIdx);

        // download new record
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadAllNewRecords(IntPtr CchexHandle, int DevIdx);

        // basic config info
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetBasicConfigInfo(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetBasicConfigInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_BASIC_CFG_INFO_STRU Config);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadEmployee2UnicodeInfo_CardIdLen7(IntPtr CchexHandle, int DevIdx, ref CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU EmployeeList, byte EmployeeNum);
        // list, modify and delete person 
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ListPersonInfo(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ModifyPersonInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_RET_PERSON_INFO_STRU personlist, byte person_num);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ModifyPersonW2Info(IntPtr CchexHandle, int DevIdx, ref CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 personlist, byte person_num);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ModifyPersonInfo_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4 personlist, byte person_num);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DeletePersonInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_DEL_PERSON_INFO_STRU Config);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DeletePersonInfo_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID Config);

        //获取单个员工资料
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetOnePersonInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_ONE_EMPLOYEE_INFO_STRU Config);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetOnePersonInfo_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_ONE_EMPLOYEE_INFO_STRU_ID_VER_4_NEWID Config);

        // get, put fp raw data
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadFingerPrint(IntPtr CchexHandle, int DevIdx, byte[] EmployeeId, byte FingerIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadFingerPrint(IntPtr CchexHandle, int DevIdx, byte[] EmployeeId, byte FingerIdx, byte[] FingerData, int DataLen);
        //
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadFingerPrint_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, byte[] EmployeeId, byte FingerIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadFingerPrint_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, byte[] EmployeeId, byte FingerIdx, byte[] FingerData, int DataLen);

        // delete record or new flag
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DeleteRecordInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_DEL_RECORD_INFO_STRU Config);






        //add~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~2.0
        
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetTime(IntPtr CchexHandle, int DevIdx);
        

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_InitUserArea(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_InitSystem(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetBasicConfigInfo2(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetBasicConfigInfo2(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF config);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPeriodTime(IntPtr CchexHandle, int DevIdx, byte SerialNumbe);//(SerialNumbe==(1--32))

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetPeriodTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_PERIOD_TIME_STRU_EXT_INF config);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetTeamInfo(IntPtr CchexHandle, int DevIdx, byte TeamNumbe);//(TeamNumbe==(0--16))

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetTeamInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_TEAM_INFO_STRU_EXT_INF config);//(TeamNumbe==(2--16))

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_AddFingerprintOnline(IntPtr CchexHandle, int DevIdx, ref CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF Param);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_AddFingerprintOnline_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF_ID_VER_4_NEWID Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_ForcedUnlock(IntPtr CchexHandle, int DevIdx, IntPtr CCHEX_FORCED_UNLOCK_STRU_EXT_INF );

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_Udp_Search_Dev(IntPtr CchexHandle);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_Udp_Set_Dev_Config(IntPtr CchexHandle, ref CCHEX_UDP_SET_DEV_CONFIG_STRU_EXT_INF config);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_ClientConnect(IntPtr CchexHandle, byte[] Ip, int Port);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_ClientDisconnect(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CChex_SetSdkConfig(IntPtr CchexHandle, int SetAutoDownload, int SetRecordflag, int SetLogFile);


        //add~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~3.0
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetInfomationCode(IntPtr CchexHandle,int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetInfomationCode(IntPtr CchexHandle, int DevIdx, byte[] Data, uint DataLen);//ANSI:10     UNICODE 20

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetBellInfo(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetBellInfo(IntPtr CchexHandle, int DevIdx, byte BellTimeNum, byte Hour, byte Min, byte FlagWeek);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetUserAttendanceStatusInfo(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetUserAttendanceStatusInfo(IntPtr CchexHandle, int DevIdx,ref CCHEX_SET_USER_ATTENDANCE_STATUS_STRU  Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ClearAdministratFlag(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetSpecialStatus(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetAdminCardnumberPassword(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetAdminCardnumberPassword(IntPtr CchexHandle, int DevIdx, byte[] Data, uint DataLen);//Data[13] DataLen == 13

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetDSTParam(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetDSTParam(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_DST_PARAM_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetDevExtInfo(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetDevExtInfo(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_DEV_EXT_INFO_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetBasicConfigInfo3(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetBasicConfigInfo3(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_BASIC_CFG_INFO3_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ConnectionAuthentication(IntPtr CchexHandle, int DevIdx, ref CCHEX_CONNECTION_AUTHENTICATION_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetRecordNumByEmployeeIdAndTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_RECORD_INFO_BY_TIME Param);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadRecordByEmployeeIdAndTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_RECORD_INFO_BY_TIME Param);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetRecordNumByEmployeeIdAndTime_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID Param);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadRecordByEmployeeIdAndTime_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetRecordInfoStatus(IntPtr CchexHandle, int DevIdx);

        // Bolid   BasiConfigInfo5
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetBasicConfigInfo5(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetBasicConfigInfo5(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_BASIC_CFG_INFO5_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetCardNo(IntPtr CchexHandle, int DevIdx);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetDevCurrentStatus(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_DEV_CURRENT_STATUS_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetServiceURL(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetServiceURL(IntPtr CchexHandle, int DevIdx, byte[] UrlBuff,uint Bufflen);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetStatusSwitch(IntPtr CchexHandle, int DevIdx,byte GroupId);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetStatusSwitch(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_STATUS_SWITCH_STRU Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetStatusSwitch_EXT(IntPtr CchexHandle, int DevIdx, byte FlagWeek);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetStatusSwitch_EXT(IntPtr CchexHandle, int DevIdx, ref CCHEX_SET_STATUS_SWITCH_STRU_EXT Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UpdateDevStatus(IntPtr CchexHandle, int DevIdx);
        // [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        // public static extern int CChex_UploadFile(IntPtr CchexHandle, int DevIdx,byte FileType,byte[] FileName,byte[] Buff, int Len);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetMachineId(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SetMachineId(IntPtr CchexHandle, int DevIdx,  uint MachineId);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_Find_DevIdx_By_MachineId(IntPtr CchexHandle, uint MachineId);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //open Connection by CCHEX_CONNECTION_AUTHENTICATION_STRU  ;if NULL: disable Connect Authentication 
        public static extern int CChex_Set_Connect_Authentication(IntPtr CchexHandle, IntPtr CCHEX_CONNECTION_AUTHENTICATION_STRU);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadRecord(IntPtr CchexHandle, int DevIdx, ref CCHEX_UPLOAD_RECORD_INFO_STRU record);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadRecord_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_UPLOAD_RECORD_INFO_STRU_VER_4_NEWID record);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_ManageLogRecord(IntPtr CchexHandle, int DevIdx, ref CCHEX_MANAGE_LOG_RECORD Param);



        /***********************************
 * 功能 : 获取照片文件总数
************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPictureNumber(IntPtr CchexHandle, int DevIdx);
        /***********************************
         * 功能 : 获取照片文件头信息
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPictureAllHeadInfo(IntPtr CchexHandle, int DevIdx);
        /***********************************
         * 功能 : 获取指定照片文件
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPictureByEmployeeIdAndTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_EID_AND_TIME Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPictureByEmployeeIdAndTime_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID Param);

        
        /***********************************
         * 功能 : 删除指定照片文件
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DelPictureByEmployeeIdAndTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_EID_AND_TIME Param);

        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DelPictureByEmployeeIdAndTime_VER_4_NEWID(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID Param);


        /***********************************
 * 功能 : 获取测温记录数     Type:0-所有类型,10-测温正常,20-测温异常      现支持型号:FACE7T    FACE7TM
************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetTRecordNumberByType(IntPtr CchexHandle, int DevIdx, int RecordTpye);
        /***********************************
         * 功能 : 根据测温类型下载记录                                          现支持型号:FACE7T    FACE7TM
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetTRecordByType(IntPtr CchexHandle, int DevIdx, int RecordTpye);
        /***********************************
         * 功能 : 获取指定测温记录ID照片文件                                    现支持型号:FACE7T    FACE7TM
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_GetPictureByTRecordIdType(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_RECORD_ID_STRU Param);
        /***********************************
         * 功能 : 删除指定测温记录ID照片文件                                    现支持型号:FACE7T    FACE7TM
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DelPictureByTRecordIdType(IntPtr CchexHandle, int DevIdx, ref CCHEX_PICTURE_BY_RECORD_ID_STRU Param);






        /*****************************************************************************
         * 机型：FACE7TM 下载口罩测温考勤记录 下载 上传 
         *****************************************************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_TM_DownloadAllRecords(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_TM_DownloadAllNewRecords(IntPtr CchexHandle, int DevIdx);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_TM_UploadRecord(IntPtr CchexHandle, int DevIdx, ref CCHEX_TM_UPLOAD_RECORD_INFO_STRU Record);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_TM_DownloadRecordByEmployeeIdAndTime(IntPtr CchexHandle, int DevIdx, ref CCHEX_GET_RECORD_INFO_BY_TIME Param);

        /***********************************
 * 功能 : 图片人脸模板-下载                                  
 * 现支持型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM
************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_DownloadFacePictureModule(IntPtr CchexHandle, int DevIdx, ref CCHEX_DEL_PERSON_INFO_STRU Param);

        /***********************************
         * 功能 : 图片人脸模板-上传                                  
         * 现支持型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM
         * PictureBuff, BuffLen:MAX: <= 512000
        ************************************/
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_UploadFacePictureModule(IntPtr CchexHandle, int DevIdx, ref CCHEX_DEL_PERSON_INFO_STRU Param, byte[] PictureBuff, int BuffLen);
 /*****************************************************************************
 * 功能:在线登记指纹 人脸模块
 * 
 * 设备型号:FDEEP5 FDEEP5T, FDEEP5M, FDEEP5TM FDEEP3 FDEEP3T, FDEEP3M, FDEEP3TM
 * 信息返回类型:CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE
 * 信息解析结构:CCHEX_DOWNLOAD_FACE_PICTURE_MODULE  
 * @brief 
 * 在线登记指纹
 * @param CchexHandle   句柄
 * @param DevIdx        设备通信号
 * @param Param         相关参数(注:在线登记时,需使用一个临时用户登记,登记完成后该临时用户将会被"删除",请输入一个非正式使用员工号)
 * @return API_EXTERN   1:命令入列成功 -1:命令入列失败
 */
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CCHex_AddFingerprintOnline_FacePicture(IntPtr CchexHandle, int DevIdx, ref CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF Param);





        //        /****************************************************************
        //         *SAC 上传 下载 员工   一次最大上传500 
        //        *****************************************************************/

        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllEmployeeInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        ///****************************************************************
        //*SAC 上传 下载 组信息   一次最大上传5
        //*****************************************************************/
        //        public static extern int CChex_SAC_UploadEmployeeInfo(IntPtr CchexHandle, int DevIdx , ref SAC_EMPLOYEE_INFO_STRU EmployeeList, uint EmployeeNum);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_GROUP_INFO_STRU GroupList, uint GroupNum);
        ///****************************************************************
        // *SAC 上传 下载 员工组合   一次最大上传1500
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllEmployeeWithGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadEmployeeWithGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_EMPLOYEE_WITH_GROUP_INFO_STRU DataList, uint DataNum);


        //        /****************************************************************
        // *SAC 设置 下载 门信息
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_GetDoorInfo(IntPtr CchexHandle, int DevIdx,byte DoorIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_SetDoorInfo(IntPtr CchexHandle, int DevIdx, ref SAC_SET_DOOR_INFO_STRU Param);
        //        /****************************************************************
        //         *SAC 上传 下载 门组信息 一次最大上传5
        //        *****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllDoorGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadDoorGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_GROUP_INFO_STRU DataList, uint DataNum);
        //        /****************************************************************
        // *SAC 上传 下载 门组合信息   一次最大上传5
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllDoorWithDoorGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadDoorWithDoorGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_DOOR_WITH_DOORGROUP_INFO_STRU DataList, uint DataNum);

        //        /****************************************************************
        // *SAC 上传 下载 时间段信息 
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadTimeFrameInfo(IntPtr CchexHandle, int DevIdx, byte TimeFrameNum);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadTimeFrameInfo(IntPtr CchexHandle, int DevIdx, ref SAC_UPLOAD_TIME_FRAME_INFO_STRU Param, byte TimeFrameNum);
        //        /****************************************************************
        // *SAC 上传 下载 时间段组信息一次最大上传5
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllTimeGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadTimeGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_GROUP_INFO_STRU DataList, uint DataNum);

        //        /****************************************************************
        // *SAC 上传 下载 时间段组合信息   一次最大上传5
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAllTimeFrameWithTimeGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadTimeFrameWithTimeGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_TimeFrame_WITH_TimeGROUP_INFO_STRU DataList, uint DataNum);

        //        /****************************************************************
        // *SAC 上传 下载 门禁时段组信息   一次最大上传5
        //*****************************************************************/
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_DownloadAccessControlGroupInfo(IntPtr CchexHandle, int DevIdx);
        //        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        //        public static extern int CChex_SAC_UploadAccessControlGroupInfo(IntPtr CchexHandle, int DevIdx, ref SAC_ACCESS_CONTROL_GROUP_INFO_STRU DataList, uint DataNum);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SAC_Download_Common(IntPtr CchexHandle, int DevIdx, uint CmdPri);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SAC_Upload_Common(IntPtr CchexHandle, int DevIdx, uint CmdPri,uint DataCount, byte[] DataBuff,uint DataLen);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SAC_Delete_Common(IntPtr CchexHandle, int DevIdx, uint CmdPri, uint DataCount, byte[] DataBuff, uint DataLen);
        [DllImport("tc-b_new_sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CChex_SAC_Init_Common(IntPtr CchexHandle, int DevIdx, uint CmdPri);

    }



}
