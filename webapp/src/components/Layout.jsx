import React from 'react';
import '../styles/layout.css'; 
import profile from '../assets/images/profile.jpg'
import { FaBell } from 'react-icons/fa'; // Importing a notification bell icon from react-icons
import { useSelector } from 'react-redux'; // Import Redux's useSelector hook

export default function Layout({ children, title }) {
  // Get the user and role from Redux store
  const user = useSelector((state) => state.user.user);
  const role = useSelector((state) => state.user.role);

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
                    <img src={profile} alt='User' className='user-photo'/>
                    <div className='user-info'>
                        <p className='user-name'>{user?.username || 'User'}</p> 
                        <p className='user-designation'>{role || 'Role'}</p> 
                    </div>
                </div>
            </div>
        </div>
        <div className="line"></div>
        <div className="body">
            {children}
        </div>   
    </div>
  );
}
