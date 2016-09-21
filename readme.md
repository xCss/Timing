# Timing 
定时请求任务工具

![xxx](https://ws2.sinaimg.cn/large/6dcfd1b8gw1f812hj1hzoj20fk084gmx.jpg)

## Development Tools
 - [Visual Studio 2012](https://www.visualstudio.com)
 - Base on [.Net FrameWrok 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=1639)

## Support System
 - 测试机型: Windows 10.0 x64 (10240)
 - 理论支持: Windows XP(SP3)+ `(any cpu)`
 
## Download
 - [Timing.rar](https://github.com/xCss/Timing/releases/download/v1.0.0/Timing.rar)

## :warning: 请注意
 - 如果`config.xml`文件中的`url`里有`&`符号，请替换成`&#038;`
 
## config.xml
```xml
<?xml version="1.0" standalone="no"?>
<links>
  <link>
    <!-- 请求链接 -->
    <url>https://bird.ioliu.cn/v1/?url=http://www.bing.com/HPImageArchive.aspx?format=js&#038;idx=0&#038;n=1</url>
    <!-- 请求间隔(s) -->
    <interval>1</interval>
    <!-- 是否请求(可选：默认true) -->
	<status>true</status>
    <!-- 最后执行时间(可选,readonly) -->
	<lasttime></lasttime>
  </link>
  <link>
    <!-- 请求链接 -->
    <url>https://bird.ioliu.cn/v1/?url=http://www.bing.com/HPImageArchive.aspx?format=js&#038;idx=0&#038;n=1</url>
    <!-- 请求间隔(s) -->
    <interval>1</interval>
    <!-- 是否请求(可选：默认true) -->
	<status>true</status>
    <!-- 最后执行时间(可选,readonly) -->
	<lasttime></lasttime>
  </link>
</links>
```