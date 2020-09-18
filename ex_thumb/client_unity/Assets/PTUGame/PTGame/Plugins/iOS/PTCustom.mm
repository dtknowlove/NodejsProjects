//
//  PTCustom.m
//  Unity-iPhone
//
//  Created by van on 2017/9/5.
//
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import <AVFoundation/AVFoundation.h>
#import <sys/utsname.h>

extern "C"
{

    void _LaunchApp(const char* contentStr){
        NSString* customURL = [NSString stringWithUTF8String:contentStr];
        if ([[UIApplication sharedApplication]
             canOpenURL:[NSURL URLWithString:customURL]])
        {
            [[UIApplication sharedApplication] openURL:[NSURL URLWithString:customURL]];
            UnitySendMessage("PTUGame", "LaunchAppCallback", "0");
        }
        else
        {
            UnitySendMessage("PTUGame", "LaunchAppCallback", "-1");

        }
    }
    
   const char* _GetAudioSessionPortType() {
        AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
        NSString *build;
        for (AVAudioSessionPortDescription* desc in [route outputs])
        {
            build = desc.portType;
        }
        if(build == nil)
        {
            build = @"error";
        }
        return strdup([build UTF8String]);
    }
    
    const char* _GetAudioSessionPortName()
    {
        AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
        NSString *build;
        for (AVAudioSessionPortDescription* desc in [route outputs])
        {
            build = desc.portName;
            break;
        }
        if(build == nil)
        {
            build = @"error";
        }
        return strdup([build UTF8String]);
    }
    
    // 获取设备型号然后手动转化为对应名称
    const char* _GetDeviceName()
    {
        // 需要#import "sys/utsname.h"
        struct utsname systemInfo;
        uname(&systemInfo);
        NSString *deviceString = [NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding];
        NSString *result=deviceString;
        
        if ([deviceString isEqualToString:@"iPhone3,1"])    result= @"iPhone 4";
        if ([deviceString isEqualToString:@"iPhone3,2"])    result= @"iPhone 4";
        if ([deviceString isEqualToString:@"iPhone3,3"])    result= @"iPhone 4";
        if ([deviceString isEqualToString:@"iPhone4,1"])    result= @"iPhone 4S";
        if ([deviceString isEqualToString:@"iPhone5,1"])    result= @"iPhone 5";
        if ([deviceString isEqualToString:@"iPhone5,2"])    result= @"iPhone 5 (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPhone5,3"])    result= @"iPhone 5c (GSM)";
        if ([deviceString isEqualToString:@"iPhone5,4"])    result= @"iPhone 5c (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPhone6,1"])    result= @"iPhone 5s (GSM)";
        if ([deviceString isEqualToString:@"iPhone6,2"])    result= @"iPhone 5s (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPhone7,1"])    result= @"iPhone 6 Plus";
        if ([deviceString isEqualToString:@"iPhone7,2"])    result= @"iPhone 6";
        if ([deviceString isEqualToString:@"iPhone8,1"])    result= @"iPhone 6s";
        if ([deviceString isEqualToString:@"iPhone8,2"])    result= @"iPhone 6s Plus";
        if ([deviceString isEqualToString:@"iPhone8,4"])    result= @"iPhone SE";
        // 日行两款手机型号均为日本独占，可能使用索尼FeliCa支付方案而不是苹果支付
        if ([deviceString isEqualToString:@"iPhone9,1"])    result= @"国行、日版、港行iPhone 7";
        if ([deviceString isEqualToString:@"iPhone9,2"])    result= @"港行、国行iPhone 7 Plus";
        if ([deviceString isEqualToString:@"iPhone9,3"])    result= @"美版、台版iPhone 7";
        if ([deviceString isEqualToString:@"iPhone9,4"])    result= @"美版、台版iPhone 7 Plus";
        if ([deviceString isEqualToString:@"iPhone10,1"])   result= @"国行(A1863)、日行(A1906)iPhone 8";
        if ([deviceString isEqualToString:@"iPhone10,4"])   result= @"美版(Global/A1905)iPhone 8";
        if ([deviceString isEqualToString:@"iPhone10,2"])   result= @"国行(A1864)、日行(A1898)iPhone 8 Plus";
        if ([deviceString isEqualToString:@"iPhone10,5"])   result= @"美版(Global/A1897)iPhone 8 Plus";
        if ([deviceString isEqualToString:@"iPhone10,3"])   result= @"国行(A1865)、日行(A1902)iPhone X";
        if ([deviceString isEqualToString:@"iPhone10,6"])   result= @"美版(Global/A1901)iPhone X";
        if ([deviceString isEqualToString:@"iPhone11,2"])   result= @"iPhone XS";
        if ([deviceString isEqualToString:@"iPhone11,4"])   result= @"iPhone XS Max";
        if ([deviceString isEqualToString:@"iPhone11,6"])   result= @"iPhone XS Max";
        if ([deviceString isEqualToString:@"iPhone11,8"])   result= @"iPhone XR";
        
        if ([deviceString isEqualToString:@"iPod1,1"])      result= @"iPod Touch 1G";
        if ([deviceString isEqualToString:@"iPod2,1"])      result= @"iPod Touch 2G";
        if ([deviceString isEqualToString:@"iPod3,1"])      result= @"iPod Touch 3G";
        if ([deviceString isEqualToString:@"iPod4,1"])      result= @"iPod Touch 4G";
        if ([deviceString isEqualToString:@"iPod5,1"])      result= @"iPod Touch (5 Gen)";
        
        if ([deviceString isEqualToString:@"iPad1,1"])      result= @"iPad";
        if ([deviceString isEqualToString:@"iPad1,2"])      result= @"iPad 3G";
        if ([deviceString isEqualToString:@"iPad2,1"])      result= @"iPad 2 (WiFi)";
        if ([deviceString isEqualToString:@"iPad2,2"])      result= @"iPad 2";
        if ([deviceString isEqualToString:@"iPad2,3"])      result= @"iPad 2 (CDMA)";
        if ([deviceString isEqualToString:@"iPad2,4"])      result= @"iPad 2";
        if ([deviceString isEqualToString:@"iPad2,5"])      result= @"iPad Mini (WiFi)";
        if ([deviceString isEqualToString:@"iPad2,6"])      result= @"iPad Mini";
        if ([deviceString isEqualToString:@"iPad2,7"])      result= @"iPad Mini (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPad3,1"])      result= @"iPad 3 (WiFi)";
        if ([deviceString isEqualToString:@"iPad3,2"])      result= @"iPad 3 (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPad3,3"])      result= @"iPad 3";
        if ([deviceString isEqualToString:@"iPad3,4"])      result= @"iPad 4 (WiFi)";
        if ([deviceString isEqualToString:@"iPad3,5"])      result= @"iPad 4";
        if ([deviceString isEqualToString:@"iPad3,6"])      result= @"iPad 4 (GSM+CDMA)";
        if ([deviceString isEqualToString:@"iPad4,1"])      result= @"iPad Air (WiFi)";
        if ([deviceString isEqualToString:@"iPad4,2"])      result= @"iPad Air (Cellular)";
        if ([deviceString isEqualToString:@"iPad4,4"])      result= @"iPad Mini 2 (WiFi)";
        if ([deviceString isEqualToString:@"iPad4,5"])      result= @"iPad Mini 2 (Cellular)";
        if ([deviceString isEqualToString:@"iPad4,6"])      result= @"iPad Mini 2";
        if ([deviceString isEqualToString:@"iPad4,7"])      result= @"iPad Mini 3";
        if ([deviceString isEqualToString:@"iPad4,8"])      result= @"iPad Mini 3";
        if ([deviceString isEqualToString:@"iPad4,9"])      result= @"iPad Mini 3";
        if ([deviceString isEqualToString:@"iPad5,1"])      result= @"iPad Mini 4 (WiFi)";
        if ([deviceString isEqualToString:@"iPad5,2"])      result= @"iPad Mini 4 (LTE)";
        if ([deviceString isEqualToString:@"iPad5,3"])      result= @"iPad Air 2";
        if ([deviceString isEqualToString:@"iPad5,4"])      result= @"iPad Air 2";
        if ([deviceString isEqualToString:@"iPad6,3"])      result= @"iPad Pro 9.7";
        if ([deviceString isEqualToString:@"iPad6,4"])      result= @"iPad Pro 9.7";
        if ([deviceString isEqualToString:@"iPad6,7"])      result= @"iPad Pro 12.9";
        if ([deviceString isEqualToString:@"iPad6,8"])      result= @"iPad Pro 12.9";
        if ([deviceString isEqualToString:@"iPad6,11"])    result= @"iPad 5 (WiFi)";
        if ([deviceString isEqualToString:@"iPad6,12"])    result= @"iPad 5 (Cellular)";
        if ([deviceString isEqualToString:@"iPad7,1"])     result= @"iPad Pro 12.9 inch 2nd gen (WiFi)";
        if ([deviceString isEqualToString:@"iPad7,2"])     result= @"iPad Pro 12.9 inch 2nd gen (Cellular)";
        if ([deviceString isEqualToString:@"iPad7,3"])     result= @"iPad Pro 10.5 inch (WiFi)";
        if ([deviceString isEqualToString:@"iPad7,4"])     result= @"iPad Pro 10.5 inch (Cellular)";
        
        if ([deviceString isEqualToString:@"AppleTV2,1"])    result= @"Apple TV 2";
        if ([deviceString isEqualToString:@"AppleTV3,1"])    result= @"Apple TV 3";
        if ([deviceString isEqualToString:@"AppleTV3,2"])    result= @"Apple TV 3";
        if ([deviceString isEqualToString:@"AppleTV5,3"])    result= @"Apple TV 4";
        
        if ([deviceString isEqualToString:@"i386"])         result= @"Simulator";
        if ([deviceString isEqualToString:@"x86_64"])       result= @"Simulator";
        
        return strdup([result UTF8String]);
    }
}

extern "C" {
    // Helper method to create C string copy
    char* MakeStringCopy (NSString* nsstring)
    {
        if (nsstring == NULL) {
            return NULL;
        }
        // convert from NSString to char with utf8 encoding
        const char* string = [nsstring cStringUsingEncoding:NSUTF8StringEncoding];
        if (string == NULL) {
            return NULL;
        }
        
        // create char copy with malloc and strcpy
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    const char* _GetSettingsURL () {
        NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
        return MakeStringCopy(url.absoluteString);
    }
    
    void _OpenSettings () {
        NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
        [[UIApplication sharedApplication] openURL: url];
    }
}
