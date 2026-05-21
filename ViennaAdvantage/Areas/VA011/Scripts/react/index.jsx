import React from 'react';
import ReactDOM from 'react-dom';

window.VA011 = window.VA011 || {};
; (function (VA011, $) {
    VA011.React = VA011.React || {};
    VA011.React = function () { }

    VA011.React.prototype.init = function (windowNo, frame, componentName) {
        const MyComponent = React.lazy(() => import(`./pages/${componentName}`));
        ReactDOM.createRoot(frame.getContentGrid()[0]).render(
            <React.Suspense fallback={<div>Loading...</div>}>
                <MyComponent windowNo={windowNo} frame={frame} />
            </React.Suspense>
        );
    };
})(VA011, jQuery);
