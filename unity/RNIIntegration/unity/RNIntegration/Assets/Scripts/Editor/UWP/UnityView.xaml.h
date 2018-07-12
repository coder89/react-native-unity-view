//
// UnityView.xaml.h
// Declaration of the UnityView class
//

#pragma once

#include "UnityView.g.h"

namespace RNUnityViewBridge
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class UnityView sealed
	{
	public:
		UnityView();

	private:
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
