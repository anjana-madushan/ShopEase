package com.sliit.shopease.helpers;

import android.content.Context;
import android.content.SharedPreferences;

public class SharedPreferencesHelper {

  private static final String PREF_NAME = "AppPreferences";
  private final SharedPreferences sharedPreferences;
  private final SharedPreferences.Editor editor;

  // Constructor
  public SharedPreferencesHelper(Context context) {
    sharedPreferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    editor = sharedPreferences.edit();
  }

  // Save String
  public void saveString(String key, String value) {
    editor.putString(key, value);
    editor.apply();
  }

  // Get String
  public String getString(String key, String defaultValue) {
    return sharedPreferences.getString(key, defaultValue);
  }

  // Save int
  public void saveInt(String key, int value) {
    editor.putInt(key, value);
    editor.apply();
  }

  // Get int
  public int getInt(String key, int defaultValue) {
    return sharedPreferences.getInt(key, defaultValue);
  }

  // Save boolean
  public void saveBoolean(String key, boolean value) {
    editor.putBoolean(key, value);
    editor.apply();
  }

  // Get boolean
  public boolean getBoolean(String key, boolean defaultValue) {
    return sharedPreferences.getBoolean(key, defaultValue);
  }

  // Save float
  public void saveFloat(String key, float value) {
    editor.putFloat(key, value);
    editor.apply();
  }

  // Get float
  public float getFloat(String key, float defaultValue) {
    return sharedPreferences.getFloat(key, defaultValue);
  }

  // Save long
  public void saveLong(String key, long value) {
    editor.putLong(key, value);
    editor.apply();
  }

  // Get long
  public long getLong(String key, long defaultValue) {
    return sharedPreferences.getLong(key, defaultValue);
  }

  // Remove specific key
  public void remove(String key) {
    editor.remove(key);
    editor.apply();
  }

  // Clear all preferences
  public void clearAll() {
    editor.clear();
    editor.apply();
  }

  // Check if a key exists
  public boolean contains(String key) {
    return sharedPreferences.contains(key);
  }
}
