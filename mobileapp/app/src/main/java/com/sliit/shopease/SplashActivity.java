package com.sliit.shopease;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import androidx.appcompat.app.AppCompatActivity;

@SuppressLint("CustomSplashScreen")
public class SplashActivity extends AppCompatActivity {
  private static final int SPLASH_DURATION = Config.SPLASH_SCREEN_DELAY;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);

    // Only show this splash screen if android < 12
    if(android.os.Build.VERSION.SDK_INT < android.os.Build.VERSION_CODES.S) {

      setContentView(R.layout.activity_splash);

      new Handler().postDelayed(() -> {
        Intent intent = new Intent(SplashActivity.this, MainActivity.class);
        startActivity(intent);
        finish();
      }, SPLASH_DURATION);
    }else{
      Intent intent = new Intent(SplashActivity.this, MainActivity.class);
      startActivity(intent);
      finish();
    }
  }
}
