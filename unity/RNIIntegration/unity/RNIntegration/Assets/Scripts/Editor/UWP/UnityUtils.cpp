
#include "pch.h"
#include "UnityUtils.h"
#include "UnityGenerated.h"

using namespace RNUnityViewBridge;

using namespace Concurrency;
using namespace Platform;
using namespace UnityPlayer;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::System::Threading;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

::UnityPlayer::AppCallbacks^ UnityUtils::m_appCallbacks = nullptr;
RNUnityViewBridge::UnityPlayer^ UnityUtils::m_player = nullptr;
Windows::ApplicationModel::Activation::SplashScreen^ UnityUtils::m_splashScreen = nullptr;

bool UnityUtils::IsInitialized::get()
{
	return m_appCallbacks != nullptr && m_appCallbacks->IsInitialized();
}

RNUnityViewBridge::UnityPlayer^ UnityUtils::Player::get()
{
	return m_player;
}

SplashScreen^ UnityUtils::SplashScreen::get()
{
	return m_splashScreen;
}

void UnityUtils::SplashScreen::set(::SplashScreen^ value)
{
	m_splashScreen = value;
}

void UnityUtils::CreatePlayer()
{
	if (m_appCallbacks == nullptr)
	{
		SetupOrientation();
		m_appCallbacks = ref new AppCallbacks();
		m_player = ref new UnityPlayer(m_appCallbacks);
	}
}

void UnityUtils::SetupOrientation()
{
	Unity::SetupDisplay();
}

void UnityUtils::PostMessage(Platform::String^ gameObject, Platform::String^ method, Platform::String^ message)
{
	m_appCallbacks->InvokeOnAppThread(ref new ::UnityPlayer::AppCallbackItem([gameObject, method, message]()
	{
		auto il2cpp = BridgeBootstrapper::GetIL2CPPBridge();
		if (il2cpp != nullptr)
		{
			BridgeBootstrapper::GetIL2CPPBridge()->onMessage(gameObject, method, message);
		}
	}), false);
}

RNUnityViewBridge::UnityPlayer::UnityPlayer(::UnityPlayer::AppCallbacks^ appCallbacks)
	: m_appCallbacks(appCallbacks)
{
	m_appCallbacks->InvokeOnAppThread(ref new ::UnityPlayer::AppCallbackItem([this]()
	{
		BridgeBootstrapper::SetDotNetBridge(this);
	}), false);
}

RNUnityViewBridge::UnityPlayer::~UnityPlayer()
{
	Quit();
}

void RNUnityViewBridge::UnityPlayer::Pause()
{
	m_appCallbacks->UnityPause(1);
}

void RNUnityViewBridge::UnityPlayer::Resume()
{
	m_appCallbacks->UnityPause(0);
}

void RNUnityViewBridge::UnityPlayer::Quit()
{
	m_appCallbacks->InvokeOnAppThread(ref new ::UnityPlayer::AppCallbackItem([this]()
	{
		BridgeBootstrapper::SetDotNetBridge(nullptr);
	}), true);

	m_appCallbacks = nullptr;
}

void RNUnityViewBridge::UnityPlayer::onMessage(Platform::String^ message)
{
	m_appCallbacks->InvokeOnUIThread(ref new ::UnityPlayer::AppCallbackItem([this, message]()
	{
		OnUnityMessage(message);
	}), false);
}
