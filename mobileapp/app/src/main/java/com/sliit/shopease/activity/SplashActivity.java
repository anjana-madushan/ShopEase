package com.sliit.shopease.activity;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.WindowManager;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;

import com.sliit.shopease.Config;
import com.sliit.shopease.R;

@SuppressLint("CustomSplashScreen")
public class SplashActivity extends AppCompatActivity {
  private static final int SPLASH_DURATION = Config.SPLASH_SCREEN_DELAY;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);

    // Only show this splash screen if android < 12
    if(android.os.Build.VERSION.SDK_INT < android.os.Build.VERSION_CODES.S) {

      getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);
      setContentView(R.layout.activity_splash);
      new Handler().postDelayed(() -> {
        Intent intent = new Intent(SplashActivity.this, MainActivity.class);
        startActivity(intent);
        finish();
      }, SPLASH_DURATION);
    }else{
      go();
    }
  }

  private void go(){
    Intent intent = new Intent(SplashActivity.this, MainActivity.class);
    startActivity(intent);
    finish();
  }
}
