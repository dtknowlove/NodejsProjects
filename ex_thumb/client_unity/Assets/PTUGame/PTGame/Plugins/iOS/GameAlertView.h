#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

#import <UIKit/UIKit.h>

@interface GameAlertView : UIViewController<UIAlertViewDelegate>

-(void)AlertMsgCancelAndSure:(NSString*)title Msg:(NSString*)msg Confirm:(NSString*)confirm Cancel:(NSString*)cancel Function:(void (^)(bool agree))Function;

-(void)AlertMsgConfirm:(NSString*)title Msg:(NSString*)msg Confirm:(NSString*)confirm Function:(void (^)(bool agree))Function;


@end

