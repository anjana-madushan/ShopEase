package com.sliit.shopease.helpers;

import static android.webkit.URLUtil.isValidUrl;

import android.content.Context;

import androidx.annotation.NonNull;

import com.google.gson.Gson;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.models.User;

import java.io.IOException;
import java.net.URISyntaxException;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import okhttp3.ResponseBody;

public class NetworkHelper {
  private static final MediaType JSON = MediaType.get("application/json; charset=utf-8");

  private static NetworkHelper instance;
  private final OkHttpClient client;

  private NetworkHelper() {
    client = new OkHttpClient.Builder()
        .connectTimeout(10, TimeUnit.SECONDS)
        .writeTimeout(10, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .build();
  }

  // Singleton pattern to ensure one instance of the NetworkHelper
  public static NetworkHelper getInstance() {
    if (instance == null) {
      instance = new NetworkHelper();
    }
    return instance;
  }

  // Perform GET Request
  public void get(Context context, String url, boolean includeToken, NetworkCallback<String> callback) {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(context);
    String baseUrl = sharedPreferencesHelper.getString(PrefKeys.BASE_URL, "");
    User user = User.fromJson(sharedPreferencesHelper.getString(PrefKeys.USER, ""));
    final String token = user.getToken();

    Request.Builder request = new Request.Builder();
    request.url(baseUrl + url);

    if(includeToken) {
      request.header("Authorization", token);
    }

    client.newCall(request.build()).enqueue(new Callback() {
      @Override
      public void onFailure(@NonNull Call call, @NonNull IOException e) {
        callback.onFailure(new ShopEaseError(e));
      }

      @Override
      public void onResponse(@NonNull Call call, @NonNull Response response) throws IOException {
        if (!response.isSuccessful()) {
          callback.onFailure(new ShopEaseError(response.code(), response.message(), response));
        } else {
          ResponseBody responseData = response.body();
          if (responseData != null) {
            callback.onSuccess(responseData.string());
          } else {
            callback.onFailure(new ShopEaseError());
          }
        }
      }
    });
  }

  // Perform POST Request
  public void post(Context context, String url, Map<String, String> jsonBody, NetworkCallback callback) {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(context);
    String baseUrl = sharedPreferencesHelper.getString(PrefKeys.BASE_URL, "");

    // Validate the URL
    if (!isValidUrl(baseUrl + url)) {
      callback.onFailure(new ShopEaseError(new URISyntaxException(baseUrl + url, "invalid Url")));
      return;
    }

    String jsonString = new Gson().toJson(jsonBody);
    RequestBody body = RequestBody.create(jsonString, JSON);
    Request request = new Request.Builder()
        .url(baseUrl + url)
        .post(body)
        .build();

    try {
      client.newCall(request).enqueue(new Callback() {
        @Override
        public void onFailure(@NonNull Call call, @NonNull IOException e) {
          e.printStackTrace();
          callback.onFailure(new ShopEaseError(e));
        }

        @Override
        public void onResponse(@NonNull Call call, @NonNull Response response) throws IOException {
          if (!response.isSuccessful()) {
            callback.onFailure(new ShopEaseError(response.code(), response.message(), response));
          } else {
            ResponseBody responseData = response.body();
            if (responseData != null) {
              callback.onSuccess(responseData.string());
            } else {
              callback.onFailure(new ShopEaseError());
            }
          }
        }
      });
    } catch (Exception e) {
      e.printStackTrace();
      callback.onFailure(new ShopEaseError(e));
    }
  }

  // Perform PUT Request
  public void put(Context context, String url, String jsonBody, NetworkCallback callback) {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(context);
    String baseUrl = sharedPreferencesHelper.getString(PrefKeys.BASE_URL, "");

    RequestBody body = RequestBody.create(jsonBody, JSON);
    Request request = new Request.Builder()
        .url(baseUrl + url)
        .put(body)
        .build();

    client.newCall(request).enqueue(new Callback() {
      @Override
      public void onFailure(@NonNull Call call, @NonNull IOException e) {
        callback.onFailure(new ShopEaseError(e));
      }

      @Override
      public void onResponse(@NonNull Call call, @NonNull Response response) throws IOException {
        if (!response.isSuccessful()) {
          callback.onFailure(new ShopEaseError(response.code(), response.message(), response));
        } else {
          ResponseBody responseData = response.body();
          if (responseData != null) {
            callback.onSuccess(responseData.string());
          } else {
            callback.onFailure(new ShopEaseError());
          }
        }
      }
    });
  }

  // Perform DELETE Request
  public void delete(Context context, String url, NetworkCallback callback) {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(context);
    String baseUrl = sharedPreferencesHelper.getString(PrefKeys.BASE_URL, "");

    Request request = new Request.Builder()
        .url(baseUrl + url)
        .delete()
        .build();

    client.newCall(request).enqueue(new Callback() {
      @Override
      public void onFailure(@NonNull Call call, @NonNull IOException e) {
        callback.onFailure(new ShopEaseError(e));
      }

      @Override
      public void onResponse(@NonNull Call call, @NonNull Response response) throws IOException {
        if (!response.isSuccessful()) {
          callback.onFailure(new ShopEaseError(response.code(), response.message(), response));
        } else {
          ResponseBody responseData = response.body();
          if (responseData != null) {
            callback.onSuccess(responseData.string());
          } else {
            callback.onFailure(new ShopEaseError());
          }
        }
      }
    });
  }
}
