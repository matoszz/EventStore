﻿using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace EventStore.Core.Telemetry;

public class MaxMetric<T> where T : struct {
	private readonly List<MaxTracker<T>> _trackers = new();

	public MaxMetric(Meter meter, string name) {
		// gauge rather than updowncounter because the dimensions wont make sense to sum,
		// because they are maxes and not necessarily from the same moment
		meter.CreateObservableGauge(name, Observe);
	}

	public void Add(MaxTracker<T> tracker) {
		lock (_trackers) {
			_trackers.Add(tracker);
		}
	}

	private IEnumerable<Measurement<T>> Observe() {
		lock (_trackers) {
			foreach (var tracker in _trackers) {
				yield return tracker.Observe();
			}
		}
	}
}
