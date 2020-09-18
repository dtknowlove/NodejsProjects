//
//  AppItunesUrl.mm
//  Unity-iPhone
//
//  Created by elang on 16/2/1.
//  Mofified by van on 16/5/17.
//
//

#import <Foundation/Foundation.h>

#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>

#include "GameAlertView.h"

extern "C"
{
    void _ShowNewAppVersionInAppStore(const char* urlStr,const char* contentStr, const char* confirmStr,const char* cancleStr)
    {
        //        UIAlertView *alert = [[UIAlertView alloc]init];
        //        [alert setTitle:@"有新版本更新"];
        //        [alert addButtonWithTitle:@"ok"];
        //        [alert show];
        //[alert release];
        
        NSURL*url =[NSURL URLWithString: [NSString stringWithUTF8String:urlStr]];
        
        if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 8.0) {//iOS8及以上
            
            [[GameAlertView new] AlertMsgCancelAndSure:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Cancel:[NSString stringWithUTF8String:cancleStr] Function:^(bool agree){
                if (agree)
                {
                    UnitySendMessage("PTUGame", "SelectUpdateApp","");
                    if([[UIApplication sharedApplication] canOpenURL:url]) {
                        
                        [[UIApplication sharedApplication] openURL:url];
                    }
                }else{
                    UnitySendMessage("PTUGame", "CancleUpdateApp","");
                }
            }];
        }
        else
        {
            [[GameAlertView new] AlertMsgConfirm:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Function:^(bool agree){
                UnitySendMessage("PTUGame", "CancelUpdateApp","");
                
            }];
        }
    }
    
    void _ShowNewAppVersionInAppStoreForeceUpdate(const char* urlStr,const char* contentStr, const char* confirmStr)
    {
        
        NSURL*url =[NSURL URLWithString: [NSString stringWithUTF8String:urlStr]];
        
        if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 8.0) {//iOS8及以上
            [[GameAlertView new] AlertMsgConfirm:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Function:^(bool agree){
                
                UnitySendMessage("PTUGame", "SelectUpdateApp","");
                if([[UIApplication sharedApplication] canOpenURL:url]) {
                    
                    [[UIApplication sharedApplication] openURL:url];
                }
            }];
        }
        else
        {
            [[GameAlertView new] AlertMsgConfirm:[NSString stringWithUTF8String:contentStr] Msg:@"" Confirm:[NSString stringWithUTF8String:confirmStr] Function:^(bool agree){
                UnitySendMessage("PTUGame", "CancelUpdateApp","");
                
            }];
        }
    }

}