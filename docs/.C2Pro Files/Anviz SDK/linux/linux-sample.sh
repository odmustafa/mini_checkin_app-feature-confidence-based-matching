rm *.o
rm linux-sample-exe

gcc -o linux-sample-exe sample.c -lpthread -lm -L ./ -l tc-b_new_sdk
#将当前路径加入到库文件搜索目录环境变量
export LD_LIBRARY_PATH=.:$LD_LIBRARY_PATH
#  ./linux-sample-exe Ip port (输入设备的IP和端口)如:
#./linux-sample-exe 192.168.19.36 5010