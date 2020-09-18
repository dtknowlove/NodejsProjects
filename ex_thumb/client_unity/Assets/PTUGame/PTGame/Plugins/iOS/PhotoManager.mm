//#import <UIKit/UIKit.h>
#import "PhotoManager.h"
#import "UnityAppController.h"
#ifdef __cplusplus
#endif

#import <AVFoundation/AVFoundation.h>


#include "GameAlertView.h"
#import <AssetsLibrary/AssetsLibrary.h>



extern "C"
{
    //打开系统相册
    void PickImageFromAlbum()
    {
        UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
        UnityAppController* uac = (UnityAppController*)[UIApplication sharedApplication].delegate;
        imagePicker.delegate = (id)uac.rootView;
        //设置选择后的图片可被编辑
        imagePicker.allowsEditing = YES;
        //imagePicker.sourceType = UIImagePickerControllerSourceTypeCamera ;
        imagePicker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        imagePicker.modalTransitionStyle = UIModalTransitionStyleCoverVertical;
        [uac.rootViewController presentViewController:imagePicker animated:YES completion:NULL];
        
    }
    
    void WriteDataAsImageToPhotosAlbumIOS(char* filename, const void* data, int dataLength)
    {
        NSData* nsdata = [NSData dataWithBytes:data length:dataLength];
        
        UIImage* imageData = [[UIImage alloc] initWithData:nsdata];
        
        CaptureCallback *callback = [[CaptureCallback alloc]init];
        
        UIImageWriteToSavedPhotosAlbum(imageData, callback, @selector(savingImageIsFinished:didFinishSavingWithError:contextInfo:), nil);
    }
	
	bool AlbumsPermissionIsOpen()
    {
        if ([[[UIDevice currentDevice] systemVersion] floatValue] >= 7.0) {
            ALAuthorizationStatus authStatus = [ALAssetsLibrary authorizationStatus];
            
            return (authStatus != ALAuthorizationStatusDenied);
        }
        else
        {
            return true;
        }
    }
    
    void OpenAlbumsPermissionSettingIOS(const char* contentStr,const char* confirmStr,const char* cancleStr)
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


@implementation CaptureCallback

- (void) savingImageIsFinished:(UIImage *)_image didFinishSavingWithError:(NSError *)_error contextInfo:(void *)_contextInfo
{
    const char* ret = _error == nullptr ? "success" : "failed";
    
    UnitySendMessage("PTUGame", "SaveImageToAlbumCallback", ret);
}

@end
