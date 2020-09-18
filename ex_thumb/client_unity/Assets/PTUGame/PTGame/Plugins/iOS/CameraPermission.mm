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
    bool cameraPermissionIsOpen()
    {
        float floatString = [[[UIDevice currentDevice] systemVersion] floatValue];
        if (floatString >= 7.0) {
            NSString *mediaType = AVMediaTypeVideo;
            AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:mediaType];
            if (authStatus == AVAuthorizationStatusDenied) {
                return false;
            }
        }
        
        return true;
    }
    
    void openCameraPermissionSettingView(const char* contentStr,const char* confirmStr,const char* cancleStr)
    {
        [[GameAlertView new] AlertMsgCancelAndSure:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Cancel:[NSString stringWithUTF8String:cancleStr] Function:^(bool agree){
            if (agree)
            {
                float floatString = [[[UIDevice currentDevice] systemVersion] floatValue];
                if (floatString >= 8.0) {
                    NSURL*url =[NSURL URLWithString:UIApplicationOpenSettingsURLString];
                    
                    if([[UIApplication sharedApplication] canOpenURL:url]) {
                        
                        [[UIApplication sharedApplication] openURL:url];
                    }
                }
            }
        }];
    }
}
