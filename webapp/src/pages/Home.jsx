import React from 'react'
import Navbar from '../components/Navbar'
import { navKeys } from '../utils/navkeys';
import Layout from '../components/Layout';

export default function Home() {

const [selectedKey, setSelectedKey] = React.useState(navKeys[0].key);

  return (
    <div style={{
        display:"flex",
        width:"100vw",
    }}>
        <Navbar getSelectedKey={(key)=>setSelectedKey(key)}/>
        {
            selectedKey === "dashboard" && <Layout title={"Home"}>
                Test
            </Layout>
        }
        {
            selectedKey === "notifications" && <Layout title={"Notifications"}></Layout>
        }

    </div>
    
  )
}
