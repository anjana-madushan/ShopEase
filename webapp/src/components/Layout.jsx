import "../styles/layout.css";
import profile from "../assets/images/profile.jpg";
import { FaBell } from "react-icons/fa"; 
import { useSelector } from "react-redux";

export default function Layout({ children, title }) {
  // Get the user and role from Redux store
  const username = useSelector((state) => state.auth.loggedUser.username);
  const role = useSelector((state) => state.auth.loggedUser.role);

  return (
    <div className="layout-container">
      <div className="title-bar">
        <div className="title">
          <p>{title}</p>
        </div>
        <div className="actions">
          <div className="notification-icon">
            <FaBell className="bell-icon" />
          </div>
          <div className="user-profile">
            <img src={profile} alt="User" className="user-photo" />
            <div className="user-info">
              <p className="user-name">{username || "User"}</p>
              <p className="user-designation">{role || "Role"}</p>
            </div>
          </div>
        </div>
      </div>
      <div className="line"></div>
      <div className="body">{children}</div>
    </div>
  );
}
