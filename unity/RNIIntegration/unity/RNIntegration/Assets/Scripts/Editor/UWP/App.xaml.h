//
// App.xaml.h
// Declaration of the App class.
//

#pragma once

#include "App.g.h"

namespace New_Unity_Project
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    ref class App sealed
    {
    protected:
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ e) override;
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ args) override;
        virtual void OnFileActivated(Windows::ApplicationModel::Activation::FileActivatedEventArgs^ args) override;

    internal:
        App();

    private:
        void InitializeUnity(Platform::String^ args);
    };
}
