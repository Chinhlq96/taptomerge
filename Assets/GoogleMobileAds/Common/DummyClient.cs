// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    internal class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdStarted = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<Reward> OnAdRewarded = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        public string UserId
        {
            get
            {
                return "UserId";
            }
            set { }
        }

        public DummyClient()
        {
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
        }

        public void LoadAd(AdRequest request)
        {
        }

        public void ShowBannerView()
        {
        }

        public void HideBannerView()
        {
        }

        public void DestroyBannerView()
        {
        }

        public void CreateInterstitialAd(string adUnitId)
        {
        }

        public bool IsLoaded()
        {
            return true;
        }

        public void ShowInterstitial()
        {
        }

        public void DestroyInterstitial()
        {
        }

        public void CreateRewardBasedVideoAd()
        {
        }

        public void SetUserId(string userId)
        {
        }

        public void LoadAd(AdRequest request, string adUnitId)
        {
        }

        public void DestroyRewardBasedVideoAd()
        {
        }

        public void ShowRewardBasedVideoAd()
        {
        }

        public void SetDefaultInAppPurchaseProcessor(IDefaultInAppPurchaseProcessor processor)
        {
        }

        public void SetCustomInAppPurchaseProcessor(ICustomInAppPurchaseProcessor processor)
        {
        }
    }
}
