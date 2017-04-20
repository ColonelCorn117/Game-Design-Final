using System;
using System.Collections;
using System.Collections.Generic;

public class Condition : GenericGameObject {

	public List<Requirement> requirement;
	public Action outcome;

	public bool Satisfied(Action a) {
		foreach (Requirement r in requirement) {
			if (r.Satisfied (a)) {
				return true;
			}
		}
		return false;
	}
}
