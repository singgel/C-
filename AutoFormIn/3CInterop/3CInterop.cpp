// dllmain.cpp : 定义 DLL 的初始化例程。
//

#include <Windows.h>
#include<sstream>
#include<CommCtrl.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
#pragma data_seg("InstCounter")
DWORD _threadId;
HWND _notifyWnd;
#pragma data_seg()

#pragma comment(linker,"/SECTION:InstCounter,RWS")

//typedef HHOOK (CALLBACK *SETWINDOWSHOOKEXW)(int,HOOKPROC,HINSTANCE,DWORD);
//SETWINDOWSHOOKEXW setWindowsHookExW = NULL;
const unsigned int NOTIFY_WM_CREATE = WM_USER + 0x8000;  
HINSTANCE instance = NULL;
HHOOK callWndHook = NULL;

BOOL __stdcall DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	if(dwReason == DLL_PROCESS_DETACH || dwReason == DLL_THREAD_DETACH)
	{
		::MessageBoxA(_notifyWnd, "正在卸载dll！", "消息", MB_OK);
	}
	instance = hInstance;
	return TRUE;
}

LRESULT __stdcall CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{  
	CWPSTRUCT* msg = (CWPSTRUCT*)lParam;
	if(msg != NULL && _notifyWnd != NULL)
	{
		switch(msg->message)
		{
		case WM_CREATE:
			::MessageBoxA(_notifyWnd, "窗口创建消息！", "消息", MB_OK);
			SendMessage(_notifyWnd, NOTIFY_WM_CREATE, (WPARAM)(msg->hwnd), NULL);
			break;
		}
	}
	return CallNextHookEx(callWndHook, nCode, wParam, lParam);
}
//
HHOOK __stdcall SetCallWndProcHook(HWND notifyWnd, DWORD threadId)
{
	_notifyWnd = notifyWnd;
	HOOKPROC hkProc = &CallWndProc;
	_threadId = threadId;
	callWndHook = SetWindowsHookEx(WH_CALLWNDPROC, hkProc, instance, threadId);
	return callWndHook;
}

BOOL __stdcall UnHookCallWndProc(HHOOK hhk)
{
	return UnhookWindowsHookEx(hhk);
}


