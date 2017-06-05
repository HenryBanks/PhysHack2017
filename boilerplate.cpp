#include "stdafx.h"
#include "Algo SDK Sample.h"
#include "NSK_Algo.h"
#include "thinkgear.h"
#include <stdio.h>
#include <string>
#include <WinUser.h>
#include <windowsx.h>
#include <Windows.h>
#include <CommCtrl.h>
#include <time.h>
#include <iostream>
#include <fstream>
#include <assert.h>
#include <cwchar>

extern "C" {
	void *myMalloc(size_t size) {
		return malloc(size);
	}
};

void * operator new (size_t size) {
	return myMalloc(size);
}

#define MAX_LOADSTRING 100

#define NSK_ALGO_CDECL(ret, func, args)     typedef ret (__cdecl *func##_Dll) args; func##_Dll func##Addr = NULL; char func##Str[] = #func;

NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_Init, (eNSK_ALGO_TYPE type, const NS_STR dataPath));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_Uninit, (NS_VOID));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_RegisterCallback, (NskAlgo_Callback cbFunc, NS_VOID *userData));
NSK_ALGO_CDECL(NS_STR, NSK_ALGO_SdkVersion, (NS_VOID));
NSK_ALGO_CDECL(NS_STR, NSK_ALGO_AlgoVersion, (eNSK_ALGO_TYPE type));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_Start, (NS_BOOL bBaseline));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_Pause, (NS_VOID));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_Stop, (NS_VOID));
NSK_ALGO_CDECL(eNSK_ALGO_RET, NSK_ALGO_DataStream, (eNSK_ALGO_DATA_TYPE type, NS_INT16 *data, NS_INT dataLenght));



char *comPortName = NULL;
int   dllVersion = 0;
int   connectionId = -1;
int   packetsRead = 0;
int   errCode = 0;
// unsigned int dwThreadId = -1;
// HANDLE threadHandle = NULL;
bool bConnectedHeadset = false;

int lSelectedAlgos = 0;
long raw_data_count = 0;
short *raw_data = NULL;
bool bRunning = false;
bool bInited = false;

#define MWM_COM "COM5"

#define EEG_RAW_DATA "ME_Easy_RawData.txt"

#ifdef _WIN64
#define ALGO_SDK_DLL L"AlgoSdkDll64.dll"
#else
#define ALGO_SDK_DLL L"AlgoSdkDll.dll"
#endif


//static unsigned int ThreadReadPacket(void *ThreadParam) {
//	int rawCount = 0;
//	short rawData[512] = { 0 };
//	int lastRawCount = 0;
//
//	while (true) {
//		/* Read a single Packet from the connection */
//		packetsRead = TG_ReadPackets(connectionId, 1);
//
//		/* If TG_ReadPackets() was able to read a Packet of data... */
//
//		if (packetsRead == 1) {
//			/* If the Packet containted a new raw wave value... */
//			if (TG_GetValueStatus(connectionId, TG_DATA_RAW) != 0) {
//				/* Get and print out the new raw value */
//				rawData[rawCount++] = (short)TG_GetValue(connectionId, TG_DATA_RAW);
//				lastRawCount = rawCount;
//				if (rawCount == 512) {
//					(NSK_ALGO_DataStreamAddr)(NSK_ALGO_DATA_TYPE_EEG, rawData, rawCount);
//					rawCount = 0;
//				}
//			}
//			if (TG_GetValueStatus(connectionId, TG_DATA_POOR_SIGNAL) != 0) {
//				short pq = (short)TG_GetValue(connectionId, TG_DATA_POOR_SIGNAL);
//				rawCount = 0;
//				(NSK_ALGO_DataStreamAddr)(NSK_ALGO_DATA_TYPE_PQ, &pq, 1);
//			}
//			if (TG_GetValueStatus(connectionId, TG_DATA_ATTENTION) != 0) {
//				short att = (short)TG_GetValue(connectionId, TG_DATA_ATTENTION);
//				(NSK_ALGO_DataStreamAddr)(NSK_ALGO_DATA_TYPE_ATT, &att, 1);
//			}
//			if (TG_GetValueStatus(connectionId, TG_DATA_MEDITATION) != 0) {
//				short med = (short)TG_GetValue(connectionId, TG_DATA_MEDITATION);
//				(NSK_ALGO_DataStreamAddr)(NSK_ALGO_DATA_TYPE_MED, &med, 1);
//			}
//		}
//		//Sleep(1);
//	}
//}


static NS_VOID nskSdkFuncCB(sNSK_ALGO_CB_PARAM param) {
	if (param.cbType == NSK_ALGO_CB_TYPE_ALGO) {
		if (param.param.index.type == NSK_ALGO_TYPE_BLINK) {
			//output code goes here.
			std::cout << param.param.index.value.group.eye_blink_strength << std::endl;
		}
		else {
			std::cout << "Huh?" << std::endl;
		}
	}
	else if (param.cbType == NSK_ALGO_CB_TYPE_SIGNAL_LEVEL) {
		switch (param.param.sq) {
		case NSK_ALGO_SQ_GOOD:
			std::cout << "Looks good" << std::endl;
		case NSK_ALGO_SQ_MEDIUM:
			std::cout << "Looks medium" << std::endl;
		case NSK_ALGO_SQ_POOR:
			std::cout << "Looks poor" << std::endl;
		case NSK_ALGO_SQ_NOT_DETECTED:
			std::cout << "Not detected" << std::endl;
		}
	}
	else {
		std::cout << "I don't know what you want" << std::endl;
	}
}


static void *getFuncAddr(HINSTANCE hinstLib, char *funcName, bool *bError) {
	void *funcPtr = (void*)GetProcAddress(hinstLib, funcName);
	*bError = true;
	if (NULL == funcPtr) {
		*bError = false;
	}
	return funcPtr;
}


static bool getFuncAddrs(HINSTANCE hinstLib) {
	bool bError;

	NSK_ALGO_InitAddr = (NSK_ALGO_Init_Dll)getFuncAddr(hinstLib, NSK_ALGO_InitStr, &bError);
	NSK_ALGO_UninitAddr = (NSK_ALGO_Uninit_Dll)getFuncAddr(hinstLib, NSK_ALGO_UninitStr, &bError);
	NSK_ALGO_RegisterCallbackAddr = (NSK_ALGO_RegisterCallback_Dll)getFuncAddr(hinstLib, NSK_ALGO_RegisterCallbackStr, &bError);
	NSK_ALGO_SdkVersionAddr = (NSK_ALGO_SdkVersion_Dll)getFuncAddr(hinstLib, NSK_ALGO_SdkVersionStr, &bError);
	NSK_ALGO_AlgoVersionAddr = (NSK_ALGO_AlgoVersion_Dll)getFuncAddr(hinstLib, NSK_ALGO_AlgoVersionStr, &bError);
	NSK_ALGO_StartAddr = (NSK_ALGO_Start_Dll)getFuncAddr(hinstLib, NSK_ALGO_StartStr, &bError);
	NSK_ALGO_PauseAddr = (NSK_ALGO_Pause_Dll)getFuncAddr(hinstLib, NSK_ALGO_PauseStr, &bError);
	NSK_ALGO_StopAddr = (NSK_ALGO_Stop_Dll)getFuncAddr(hinstLib, NSK_ALGO_StopStr, &bError);
	NSK_ALGO_DataStreamAddr = (NSK_ALGO_DataStream_Dll)getFuncAddr(hinstLib, NSK_ALGO_DataStreamStr, &bError);

	return bError;
}

int main() {
	HINSTANCE hinstLib = LoadLibrary(ALGO_SDK_DLL);

	if (getFuncAddrs(hinstLib) == false) {
		FreeLibrary(hinstLib);
		return false;
	};



	connectionId = TG_GetNewConnectionId();
	if (connectionId < 0) {
		std::cout << "Failed to new connection ID" << std::endl; 
		return -1;
	}
	else {
		/* Attempt to connect the connection ID handle to serial port "COM5" */
		/* NOTE: On Windows, COM10 and higher must be preceded by \\.\, as in
		*       "\\\\.\\COM12" (must escape backslashes in strings).  COM9
		*       and lower do not require the \\.\, but are allowed to include
		*       them.  On Mac OS X, COM ports are named like
		*       "/dev/tty.MindSet-DevB-1".
		*/
		comPortName = "\\\\.\\" MWM_COM;
		errCode = TG_Connect(connectionId,
			comPortName,
			TG_BAUD_57600,
			TG_STREAM_PACKETS);
		if (errCode < 0) {
			std::cout << "There was an error yo." << std::endl;
			return -1;
		}
		else {
			bConnectedHeadset = true;
		}
	}
	lSelectedAlgos |= NSK_ALGO_TYPE_BLINK;
	eNSK_ALGO_RET ret = NSK_ALGO_RegisterCallback(&nskSdkFuncCB, NULL);
	assert(ret == NSK_ALGO_RET_SUCCESS);
	ret = NSK_ALGO_Init((eNSK_ALGO_TYPE)(lSelectedAlgos), "./");
	assert(ret == NSK_ALGO_RET_SUCCESS);
	ret = NSK_ALGO_Start(NS_FALSE);
	assert(ret == NSK_ALGO_RET_SUCCESS);
	time_t endwait;
	time_t start = time(NULL);
	time_t seconds = 10; // end loop after this time has elapsed

	endwait = start + seconds;

	printf("start time is : %s", ctime(&start));

	while (start < endwait)
	{
		//
	}

	printf("end time is %s", ctime(&endwait));
	ret = NSK_ALGO_Stop();
	assert(ret == NSK_ALGO_RET_SUCCESS);
	ret = NSK_ALGO_Uninit();
	assert(ret == NSK_ALGO_RET_SUCCESS);
	TG_Disconnect(connectionId);
	TG_FreeConnection(connectionId);
	return 0;
}
