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
//    - (void)notSupportSystemVerionTipsView
//    {
//        UIAlertView *alert = [[UIAlertView alloc]
//                              initWithTitle:@"The current system version is too low , the product can not normally experience !"
//                              message:@"Product Support IOS7.0 and above"
//                              delegate:self
//                              cancelButtonTitle:@"OK"
//                              otherButtonTitles:nil];
//        
//        alert.tag = NOT_SUPPORT_VERSION_VIEW_TAG;
//        [alert show];
//    }
    
    void ShowNotSupportSystemVersionTipsView(const char* contentStr,const char* msgStr,const char* confirmStr)
    {
        [[GameAlertView new] AlertMsgConfirm:[NSString stringWithUTF8String:contentStr] Msg:[NSString stringWithUTF8String:msgStr] Confirm:[NSString stringWithUTF8String:confirmStr] Function:^(bool agree){
            if (agree)
            {
                UnitySendMessage("PTUGame", "OnSystemNotSupportConfirm", "");
                
            }
        }];
    }
}