import React from 'react';
import '../styles/navbar.css'; 
import { navKeys } from '../utils/navkeys';
import IconRenderer from './IconRenderer'; 
import logo from '../assets/images/logo.png';

export default function Navbar({ getSelectedKey, getSelectedSubKey }) {
  const [selectedKey, setSelectedKey] = React.useState(navKeys[0].key);
  const [selectedSubKey, setSelectedSubKey] = React.useState(null);

  const handleMainPageClick = (navKey) => {
    setSelectedKey(navKey.key);
    setSelectedSubKey(null); 
    getSelectedKey(navKey.key); 
  };

  const handleSubPageClick = (subPage) => {
    setSelectedSubKey(subPage.key);
    getSelectedSubKey(subPage.key); 
  };

  return (
    <div className="navbar-container">
      {/* Logo and Title */}
      <div className="navbar-logo">
        <img src={logo} alt="System Logo" className="logo-image" />
        <span className="navbar-title">Shop Easy</span>
      </div>

      {navKeys.map((navKey) => (
        <div key={navKey.key}>
          <div
            className={`nav-item ${selectedKey === navKey.key ? 'selected' : ''}`}
            onClick={() => handleMainPageClick(navKey)}
          >
            <div className="icon">
              <IconRenderer iconKey={navKey.icon} />
            </div>
            <div className="text">{navKey.name}</div>
          </div>

          {selectedKey === navKey.key && navKey.subPages.length > 0 && (
            <div className="sub-page-container">
              {navKey.subPages.map((subPage) => (
                <div
                  key={subPage.key}
                  className={`sub-page-item ${selectedSubKey === subPage.key ? 'selected' : ''}`}
                  onClick={() => handleSubPageClick(subPage)}
                >
                  {subPage.name}
                </div>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
