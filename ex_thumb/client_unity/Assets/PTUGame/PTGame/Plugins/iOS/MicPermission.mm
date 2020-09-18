//
//  CameraPermission.m
//  Unity-iPhone
//
//  Created by elang on 15/12/30.
//
//

#import <Foundation/Foundation.h>

#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>
//#include <myplanet/myplanet.h>

#include "GameAlertView.h"

extern "C"
{
    /*
    void checkMicOpen()
    {
        [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) { 
            if (granted) { 
                // 用户同意获取数据 
                UnitySendMessage("Btn_Record", "micOK", "");
            } else { 
                // 可以显示一个提示框告诉用户这个app没有得到允许？ 
                UnitySendMessage("Btn_Record", "micFail", "");
            } 
        }];
    }*/

     bool micPermissionIsOpen()
    {
        float floatString = [[[UIDevice currentDevice] systemVersion] floatValue];
        if (floatString >= 8.0) {
            AVAudioSession *session = [AVAudioSession sharedInstance];
            AVAudioSessionRecordPermission sessionRecordPermission = [session recordPermission];
            if(sessionRecordPermission == AVAudioSessionRecordPermissionDenied){
                return false;
            }
        }else {
            __block bool bCanRecord = false;
            AVAudioSession *audioSession = [AVAudioSession sharedInstance];
            if ([audioSession respondsToSelector:@selector(requestRecordPermission:)]) {
                [audioSession performSelector:@selector(requestRecordPermission:) withObject:^(BOOL granted) {
                    if (granted) {
                        bCanRecord = true;
                    } else {
                        bCanRecord = false;
                    }
                }];
            }
            return bCanRecord;
        }

        return true;
    }
   
    
    void openMicPermissionSettingView(const char* contentStr,const char* confirmStr,const char* cancleStr)
    {
        [[GameAlertView new] AlertMsgCancelAndSure:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Cancel:[NSString stringWithUTF8String:cancleStr] Function:^(bool agree){
            if (agree)
            {
                float floatString = [[[UIDevice currentDevice] systemVersion] floatValue];
                if (floatString >= 7.0) {
                    NSURL*url =[NSURL URLWithString:UIApplicationOpenSettingsURLString];
                    
                    if([[UIApplication sharedApplication] canOpenURL:url]) {
                        
                        [[UIApplication sharedApplication] openURL:url];
                    }
                }
                
            }
        }];
    }
}
