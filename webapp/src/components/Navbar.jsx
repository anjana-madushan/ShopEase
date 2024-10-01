import React from 'react';
import '../styles/navbar.css'; // Make sure this file is in the correct directory
import { navKeys } from '../utils/navkeys';
import IconRenderer from './IconRenderer'; // Import IconRender

export default function Navbar({ getSelectedKey }) {
  const [selectedKey, setSelectedKey] = React.useState(navKeys[0].key);

  return (
    <div className="navbar-container">
      {navKeys.map((navKey) => {
        return (
          <div
            key={navKey.key}
            className={`nav-item ${selectedKey === navKey.key ? 'selected' : ''}`}
            onClick={() => {
              setSelectedKey(navKey.key);
              getSelectedKey(navKey.key);
            }}
          >
            <div className="icon">
              <IconRenderer iconKey={navKey.icon} /> {/* Use IconRender here */}
            </div>
            <div className="text">{navKey.name}</div>
            {navKey.badge && <div className="badge">{navKey.badge}</div>}
          </div>
        );
      })}
    </div>
  );
}
