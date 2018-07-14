//
// UnityView.xaml.h
// Declaration of the UnityView class
//

#pragma once

namespace UnityBridge
{
	public ref class UnityView sealed : Windows::UI::Xaml::Controls::UserControl
	{
	public:
		UnityView();

	private:
		Windows::UI::Xaml::Controls::SwapChainPanel^ m_DXSwapChainPanel;
		Windows::UI::Xaml::Controls::Grid^ m_ExtendedSplashGrid;
		Windows::UI::Xaml::Controls::Image^ m_ExtendedSplashImage;

		Windows::ApplicationModel::Activation::SplashScreen^ m_SplashScreen;
		Windows::Foundation::Rect m_SplashImageRect;
		Windows::Foundation::EventRegistrationToken m_SplashScreenRemovalEventToken;
		Windows::Foundation::EventRegistrationToken m_OnResizeRegistrationToken;

		~UnityView();

		void OnResize();
		void PositionImage();
		void GetSplashBackgroundColor(Windows::UI::Core::CoreDispatcher^ dispatcher);
		void RemoveSplashScreen();
	};
}
