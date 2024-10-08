package com.sliit.shopease.activity;

import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.UserRepo;

public class UpdatePasswordActivity extends AppCompatActivity {
  private final UserRepo userRepo = new UserRepo();
  private EditText update_pass_edt_password;
  private EditText update_pass_edt_confirm_password;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_update_password);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBarsInsets = insets.getInsets(WindowInsetsCompat.Type.systemBars());

      // Apply system bar insets (status and navigation bar)
      v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, 0);

      // Handle the keyboard (IME) inset
      Insets imeInsetsType = insets.getInsets(WindowInsetsCompat.Type.ime());
      if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
        // Keyboard is visible, add padding to avoid overlapping
        v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, imeInsetsType.bottom);
      }

      return insets;
    });

    Button update_pass_btn_update = findViewById(R.id.update_pass_btn_update);

    update_pass_edt_password = findViewById(R.id.update_pass_edt_password);
    update_pass_edt_confirm_password = findViewById(R.id.update_pass_edt_confirm_password);

    update_pass_btn_update.setOnClickListener(this::updatePassword);
  }

  private void updatePassword(View v) {
    update_pass_edt_password.clearFocus();
    update_pass_edt_confirm_password.clearFocus();

    //remove keyboard
    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    String password = update_pass_edt_password.getText().toString();
    String confirmPassword = update_pass_edt_confirm_password.getText().toString();

    if (password.isEmpty() || confirmPassword.isEmpty()) {
      DialogHelper.showAlert(this, "Error", "Please enter password");
      return;
    }

    if (!password.equals(confirmPassword)) {
      DialogHelper.showAlert(this, "Error", "Passwords do not match");
      return;
    }

    userRepo.updatePassword(this, password, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {

      }

      @Override
      public void onFailure(ShopEaseError error) {

      }
    });
  }
}