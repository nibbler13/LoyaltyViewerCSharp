﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace LoyaltyViewerWpf {
	public class FaderFrame : Frame {
		#region FadeDuration

		public static readonly DependencyProperty FadeDurationProperty =
			DependencyProperty.Register("FadeDuration", typeof(Duration), typeof(FaderFrame),
				new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(1))));

		/// <summary>
		/// FadeDuration will be used as the duration for Fade Out and Fade In animations
		/// </summary>
		public Duration FadeDuration {
			get { return (Duration)GetValue(FadeDurationProperty); }
			set { SetValue(FadeDurationProperty, value); }
		}

		#endregion
		


		private bool _allowDirectNavigation = false;
		private ContentPresenter _contentPresenter = null;
		private NavigatingCancelEventArgs _navArgs = null;

		public FaderFrame() : base() {
			// watch for navigations
			Navigating += OnNavigating;
		}

		public override void OnApplyTemplate() {
			// get a reference to the frame's content presenter
			// this is the element we will fade in and out
			_contentPresenter = GetTemplateChild("PART_FrameCP") as ContentPresenter;
			base.OnApplyTemplate();
		}

		protected void OnNavigating(object sender, NavigatingCancelEventArgs e) {
			// if we did not internally initiate the navigation:
			//   1. cancel the navigation,
			//   2. cache the target,
			//   3. disable hittesting during the fade, and
			//   4. fade out the current content
			if (Content != null && !_allowDirectNavigation && _contentPresenter != null) {
				e.Cancel = true;
				_navArgs = e;
				_contentPresenter.IsHitTestVisible = false;
				DoubleAnimation da = new DoubleAnimation(0.0d, FadeDuration);
				da.DecelerationRatio = 1.0d;
				da.Completed += FadeOutCompleted;
				_contentPresenter.BeginAnimation(OpacityProperty, da);
			}
			_allowDirectNavigation = false;
		}

		private void FadeOutCompleted(object sender, EventArgs e) {
			// after the fade out
			//   1. re-enable hittesting
			//   2. initiate the delayed navigation
			//   3. invoke the FadeIn animation at Loaded priority
			(sender as AnimationClock).Completed -= FadeOutCompleted;
			if (_contentPresenter != null) {
				_contentPresenter.IsHitTestVisible = true;

				_allowDirectNavigation = true;
				switch (_navArgs.NavigationMode) {
					case NavigationMode.New:
						if (_navArgs.Uri == null) {
							NavigationService.Navigate(_navArgs.Content);
						} else {
							NavigationService.Navigate(_navArgs.Uri);
						}
						break;

					case NavigationMode.Back:
						NavigationService.GoBack();
						break;

					case NavigationMode.Forward:
						NavigationService.GoForward();
						break;

					case NavigationMode.Refresh:
						NavigationService.Refresh();
						break;
				}

				Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
					(ThreadStart)delegate () {
						DoubleAnimation da = new DoubleAnimation(1.0d, FadeDuration);
						da.AccelerationRatio = 1.0d;
						_contentPresenter.BeginAnimation(OpacityProperty, da);
					});
			}
		}
	}
}
