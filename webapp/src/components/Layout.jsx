import React from 'react'

export default function Layout({children,title}) {
  return (
    <div>
        <h1>{title}</h1>
     <div>
        {children}
    </div>   
    </div>
  )
}
