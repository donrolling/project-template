using Common.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.BaseClasses {
	public abstract class LoggingWorker {
		public ILogger Logger { get; }

		public LoggingWorker(ILoggerFactory loggerFactory) {
			this.Logger = LogUtility.GetLogger(loggerFactory, this.GetType());
		}
	}
}