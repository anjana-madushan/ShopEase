package com.sliit.shopease.helpers;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.text.InputType;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.sliit.shopease.R;

public class DialogHelper {

  // Method to show a simple alert dialog
  public static void showAlert(Context context, String title, String message) {
    AlertDialog.Builder builder = new AlertDialog.Builder(context);
    builder.setTitle(title)
        .setMessage(message)
        .setPositiveButton("OK", (dialog, which) -> dialog.dismiss());

    // Create and show the dialog
    AlertDialog dialog = builder.create();
    dialog.show();
  }

  // Method to show an alert dialog with a callback for button clicks
  public static void showAlertWithCallback(Context context, String title, String message, final DialogCallback callback) {
    AlertDialog.Builder builder = new AlertDialog.Builder(context);
    builder.setTitle(title);
    builder.setMessage(message);
    builder.setPositiveButton("Yes", (dialogInterface, i) -> callback.onOk(null));
    builder.setNegativeButton("No", (dialogInterface, i) -> callback.onCancel());

    // Create and show the dialog
    AlertDialog dialog = builder.create();
    dialog.show();
  }

  // Method to show a editText dialog
  public static void showEdtTextDialog(Context context, String title, String defaultText, final DialogCallback callback) {
    // Inflate the custom dialog layout
    LayoutInflater inflater = LayoutInflater.from(context);
    View dialogView = inflater.inflate(R.layout.layout_dialog_input, null);

    // Initialize the EditText inside the dialog
    EditText editText = dialogView.findViewById(R.id.editTextInput);
    editText.setText(defaultText);  // Set default text if provided
    editText.setInputType(InputType.TYPE_CLASS_TEXT);

    // Build the AlertDialog
    AlertDialog.Builder builder = new AlertDialog.Builder(context);
    builder.setTitle(title);
    builder.setView(dialogView);

    // Set up the OK button
    builder.setPositiveButton("OK", (dialog, which) -> {
      String inputText = editText.getText().toString();
      if (!inputText.isEmpty()) {
        callback.onOk(inputText);  // Send the input back through callback
      } else {
        Toast.makeText(context, "Input cannot be empty", Toast.LENGTH_SHORT).show();
      }
    });

    // Set up the Cancel button
    builder.setNegativeButton("Cancel", (dialog, which) -> {
      dialog.dismiss(); // Close the dialog
      callback.onCancel(); // Handle cancellation
    });

    // Show the dialog
    builder.create().show();
  }

  // Method to show a loading dialog
  private static Dialog loadingDialog;
  public static void showLoading(Context context, String message) {
    hideLoading();
    // Create a new loading dialog if it doesn't already exist
    if (loadingDialog == null) {
      loadingDialog = new Dialog(context);
      loadingDialog.setCancelable(false); // Disable dismiss on touch outside

      // Inflate the custom loading layout
      View loadingView = LayoutInflater.from(context).inflate(R.layout.layout_dialog_loading, null);
      loadingDialog.setContentView(loadingView);

      // Set loading message
      TextView loadingMessage = loadingView.findViewById(R.id.loading_message);
      loadingMessage.setText(message);
    }
    loadingDialog.show(); // Show the loading dialog
  }

  // Method to hide the loading overlay
  public static void hideLoading() {
    if (loadingDialog != null && loadingDialog.isShowing()) {
      loadingDialog.dismiss();
      loadingDialog = null;
    }
  }

  // Interface for callback to handle input
  public interface DialogCallback {
    void onOk(String inputText);  // Called when OK is clicked

    void onCancel();              // Called when Cancel is clicked
  }
}
