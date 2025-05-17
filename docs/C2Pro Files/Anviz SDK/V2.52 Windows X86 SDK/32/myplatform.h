#ifndef _MY_PLATFORM_CONFIG_H_
#define _MY_PLATFORM_CONFIG_H_

// ************************ Platform Selection ************************  



//  #define MYPLATFORM_XCODE 1
//  #define MYPLATFORM_LINUX 1
//  #define MYPLATFORM_IOS 1


#ifdef __IPHONE_OS_VERSION_MIN_REQUIRED
#define MY_PLATFORM_STR "IOS"

    #define MYPLATFORM_XCODE 1
    #define MYPLATFORM_LINUX 1
    //#define MYPLATFORM_IOS 1
    #define MYPLATFORM_OSX 1

#endif

#ifdef __MAC_OS_X_VERSION_MIN_REQUIRED
    #define MY_PLATFORM_STR "OSX"
    #define MYPLATFORM_XCODE 1
    #define MYPLATFORM_LINUX 1
    #define MYPLATFORM_IOS 1
    //#define MYPLATFORM_OSX 1

#endif


#ifdef linux
    #define MY_PLATFORM_STR "LINUX"
    //#define MYPLATFORM_XCODE 1
    #define MYPLATFORM_LINUX 1
    //#define MYPLATFORM_IOS 1
    //#define MYPLATFORM_OSX 1
#endif

#ifdef _UNIX
    #define MY_PLATFORM_STR "UNIX"
    //#define MYPLATFORM_XCODE 1
    #define MYPLATFORM_LINUX 1
    //#define MYPLATFORM_IOS 1
    //#define MYPLATFORM_OSX 1

#endif

#ifdef __WINDOWS_
    #define MY_PLATFORM_STR "WIN32"
    #define MYPLATFORM_WIN32 1

#endif

#ifdef _MSC_VER
    #define MY_PLATFORM_STR "WIN32"
    #define MYPLATFORM_WIN32 1
#endif


#ifdef _WIN32
    #define MY_PLATFORM_STR "WIN32"
    #define MYPLATFORM_WIN32 1
#endif








//for sdl

#ifndef MYPLATFORM_XCODE
#define _PLAY_IMPL_EXPORT_
#endif

#endif
