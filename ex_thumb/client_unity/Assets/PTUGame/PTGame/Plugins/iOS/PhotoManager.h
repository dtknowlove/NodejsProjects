#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#ifdef __cplusplus
#endif


@interface CaptureCallback : NSObject

- (void) savingImageIsFinished:(UIImage *)_image didFinishSavingWithError:(NSError *)_error contextInfo:(void *)_contextInfo;

@end