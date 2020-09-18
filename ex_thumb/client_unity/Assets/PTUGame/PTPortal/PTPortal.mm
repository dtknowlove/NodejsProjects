//
//  PTCustom.m
//  Unity-iPhone
//
//  Created by van on 2018/10/10.
//
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import <AVFoundation/AVFoundation.h>


extern "C" {
    // Helper method to create C string copy
    void _PortalFinish (const char* skuId,const char* configName)
    {
        
    }
    
    void _PortalFinishClickDone (const char* skuId,const char* configName)
    {
        
    }
    
    void _PortalReady()
    {
        
    }
    void _SetActivedSkus(const char* activedList)
    {
        
    }
    
    // unity 资源更新完成调用
    void _CompleteResUpdate()
    {
        
    }
    //unity 是否有资源需要去更新
    void _SetHasNewResToUpdate(bool value)
    {
//        value=true;
//        value = false;
    }
    
    const char* _GetChildInfo(){
        
        NSString *childInfo = @"1#1#1";
        // appid#appcid#apptoken
        return strdup([childInfo UTF8String]);;
    }
    
    const  bool _GuideFinish()
    {
        return false;
    }
    
    //友盟数据打点
    void _StartLevel (const char* levelId)
     {
                                      
     }
     //友盟数据打点
    void _FinishLevel (const char* levelId)
    {
        
    }
     //友盟数据打点
    void _PageBegin (const char* pageName)
    {
        
    }
     //友盟数据打点
    void _EndPage (const char* pageName)
    {
        
    }
    
    
    // UnitySendMessage("PTUGame", "SetMusicState", "true");
    // UnitySendMessage("PTUGame", "SetMusicState", "false");
    // UnitySendMessage("UIPortalMenu", "PortalToBlockBots", "");
    // UnitySendMessage("PTUGame", "GetActivedSkuList", "");
    

}
