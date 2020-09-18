#import "GameAlertView.h"

void (^alertFunction) (bool agree);



@implementation GameAlertView

-(void)AlertMsgCancelAndSure:(NSString*)title Msg:(NSString*)msg Confirm:(NSString*)confirm Cancel:(NSString*)cancel Function:(void (^)(bool agree))Function{
    if (!title&&!msg) {
        return;
    }
    if (Function) {
        alertFunction = Function;
    }
    if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 8.0) {//iOS8及以上
        UIAlertController *actionSheetController = [UIAlertController alertControllerWithTitle:title message:msg preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *alertAction=[UIAlertAction actionWithTitle:cancel style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            if(Function)
                Function(false);
        }];
        [actionSheetController addAction:alertAction];
        
        alertAction = [UIAlertAction actionWithTitle:confirm style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            if (Function) {
                (Function)(true);
            }
        }];
        
        [actionSheetController addAction:alertAction];
        UIWindow *keywindow = [[UIApplication sharedApplication] keyWindow];
        [keywindow.rootViewController presentViewController:actionSheetController animated:YES completion:nil];
    }else{
        UIAlertView *alertView=[[UIAlertView alloc] initWithTitle:title message:msg delegate:self cancelButtonTitle:cancel otherButtonTitles:confirm, nil];
        alertView.delegate=self;
        [alertView show];
    }
    
}

-(void)AlertMsgConfirm:(NSString*)title Msg:(NSString*)msg Confirm:(NSString*)confirm Function:(void (^)(bool agree))Function{
    if (!title&&!msg) {
        return;
    }
    if (Function) {
        alertFunction = Function;
    }
    if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 8.0) {//iOS8及以上
        UIAlertController *actionSheetController = [UIAlertController alertControllerWithTitle:title message:msg preferredStyle:UIAlertControllerStyleAlert];
        
        UIAlertAction *alertAction = [UIAlertAction actionWithTitle:confirm style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            if (Function) {
                (Function)(true);
            }
        }];
        
        [actionSheetController addAction:alertAction];
        UIWindow *keywindow = [[UIApplication sharedApplication] keyWindow];
        [keywindow.rootViewController presentViewController:actionSheetController animated:YES completion:nil];
    }else{
        UIAlertView *alertView=[[UIAlertView alloc] initWithTitle:title message:msg delegate:self cancelButtonTitle:confirm otherButtonTitles:nil, nil];
        alertView.delegate=self;
        [alertView show];
    }
    
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{
        if (alertFunction) {
            (alertFunction)(buttonIndex==0?false:true);
            alertFunction = nil;
        }
}

@end
