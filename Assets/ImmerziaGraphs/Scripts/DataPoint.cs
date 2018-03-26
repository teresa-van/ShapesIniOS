using GraphLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint : MonoBehaviour
{
	public Dictionary<AttributeType, object> Values { get; private set; }
	public Dictionary<string, string> Data { get; private set; }
	static MaterialPropertyBlock properties;
	Renderer renderer;

	void Awake()
	{
		Values = new Dictionary<AttributeType, object>();
		Data = new Dictionary<string, string>();
		renderer = GetComponent<Renderer>();

		if (properties == null)
			properties = new MaterialPropertyBlock();
	}

	public DataPoint SetValues(Dictionary<AttributeType, object> values)
	{
		Values = values;
		return this;
	}

	public DataPoint SetData(Dictionary<string, string> data)
	{
		Data = data;
		return this;
	}

	public void SetColor(Color color)
	{
		//((SpriteRenderer)renderer).color = color;
		properties.SetColor("_Color", color);
		renderer.SetPropertyBlock(properties);
	}
}
