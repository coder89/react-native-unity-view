#pragma once

namespace RNUnityViewBridge
{
	ref class UnityPlayer;

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class UnityUtils sealed
	{
		static RNUnityViewBridge::UnityPlayer^ m_player;
		static ::UnityPlayer::AppCallbacks^ m_appCallbacks;
		static Windows::ApplicationModel::Activation::SplashScreen^ m_splashScreen;

	private:
		UnityUtils() {};

	public:
		static property bool IsInitialized
		{
			bool get();
		}

		static property RNUnityViewBridge::UnityPlayer^ Player
		{
			RNUnityViewBridge::UnityPlayer^ get();
		}

		static property Windows::ApplicationModel::Activation::SplashScreen^ SplashScreen
		{
			Windows::ApplicationModel::Activation::SplashScreen^ get();
			void set(Windows::ApplicationModel::Activation::SplashScreen^ value);
		}

		static void CreatePlayer();
		static void SetupOrientation();
		static void PostMessage(Platform::String^ gameObject, Platform::String^ method, Platform::String^ message);
	};

	[Windows::Foundation::Metadata::WebHostHidden]
	public delegate void OnUnityMessageDelegate(Platform::String^ message);

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class UnityPlayer sealed : IDotNetBridge
	{
		::UnityPlayer::AppCallbacks^ m_appCallbacks;

	public:
		UnityPlayer(::UnityPlayer::AppCallbacks^ appCallbacks); 
		virtual ~UnityPlayer();

		event OnUnityMessageDelegate^ OnUnityMessage;

		void Pause();
		void Resume();
		void Quit();

		virtual void onMessage(Platform::String^ message);
	};
}
