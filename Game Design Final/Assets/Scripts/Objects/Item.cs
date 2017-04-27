using System.Xml;
using System.Xml.Serialization;

[XmlRoot("item")]
public class Item : GenericGameObject {

	public Action action;
	int unclaimed = 1;
	int consumed = 0;
	public int exists;

	public Item() {
		action = new Action ();
	}

	public Item(string n) {
		this.name = n;
		action = new Action ();

		action.itemGained = n;
		exists = 1;
	}

	public void setName(string n) {
		this.name = n;
		action.itemGained = n;
	}

	public bool isClaimed() {
		return (this.consumed == 1) || Controller_Game.ctrl_game.itemList.Contains (this.name);
		//return unclaimed == 0;
	}

	public void claim() {
		unclaimed = 0;
	}

	public void consume() {
		consumed = 1;
	}

	public bool isConsumed() {
		return consumed == 1;
	}

	public bool possessed() {
		return Controller_Game.ctrl_game.itemList.Contains (this.name);
		//return (unclaimed == 0 && consumed != 0);
	}
}
