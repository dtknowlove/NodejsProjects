extern "C" {
    
    const char * _GetCFBundleVersion()
    {
        NSString *version = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleShortVersionString"];
        return strdup([version UTF8String]);
    }
    
    const char* GetBundleBuildIOS()
    {
        NSString *build = [[[NSBundle mainBundle] infoDictionary] objectForKey:(NSString *)kCFBundleVersionKey];
        
        return strdup([build UTF8String]);
    }
}
