import { configureStore } from "@reduxjs/toolkit";
import { persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";
import authSlice from "./auth/authSlice";
import orderSlice from "./order/orderSlice";


const persistConfig = {
  key: "root",
  storage,
};

const persistedReducer = persistReducer(persistConfig, authSlice);
const persistedOrderReducer = persistReducer(persistConfig, orderSlice);

const store = configureStore({
  reducer: {
    auth: persistedReducer,
    order: persistedOrderReducer,
  },
});

export default store;
