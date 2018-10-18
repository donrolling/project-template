using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Interfaces {
	public interface IEntity<T> where T : struct {
		T Id { get; set; }
	}
}