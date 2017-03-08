using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityORM;

public class Device
{
	public int id;
	public string udid;
	public string userId;
	public string appVersion;
	public string os;
	public string osVersion;
	public string model;
	public string locale;
	public string timeZone;
	public string type;
	public int flag;
	public string ownerName;
	public string pushToken;
	public string note;

	public DateTime createdTime;
	public DateTime updatedTime;

	public static Device load (object obj)
	{
		return JSONMapper.DefaultJsonMapper.ReadFromJSONObject <Device> (obj) [0];
	}

	public override string ToString ()
	{
		return JSONMapper.DefaultJsonMapper.Write<Device> (this);
	}

	public static Dictionary<string, object> getParams ()
	{
		Dictionary<string, object> j = new Dictionary<string, object> ();
		j ["model"] = SystemInfo.deviceModel;
		j ["uuid"] = SystemInfo.deviceUniqueIdentifier;
		j ["platform"] = SystemInfo.deviceType.ToString ();
		j ["ownerName"] = SystemInfo.deviceName;
		j ["osVersion"] = SystemInfo.operatingSystem;
		j ["appVersion"] = TrackedBundleVersion.Current.version;
		j ["bundleIdentifier"] = TrackedBundleVersion.bundleIdentifier;
		j ["locale"] = Application.systemLanguage;
		return j;
	}

}
