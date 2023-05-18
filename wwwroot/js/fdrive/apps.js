class FenrusDriveApps
{
    initDone = false;
    constructor(){
        this.divLaunchingApp = document.getElementById('launching-app');
        this.container = document.getElementById('apps-actual');
        this.eleIframeContainer = document.createElement('div');
        this.eleIframeContainer.innerHTML = '<div class="browser-container">' +
            '  <div class="browser-header">' +
            '    <div class="browser-address-bar">' +
            '      <img />' +
            '      <input type="text" readonly>' +  
            '    </div>' +
            '    <div class="browser-controls">' +
            '      <button class="open-new-tab"><i class="fa-solid fa-arrow-up-right-from-square"></i></button>' +
            '      <button class="close-button"><i class="fa-solid fa-xmark"></i></button>' +
            '    </div>' +
            '  </div>' +
            '  <div class="browser-iframe-container">' +
            '    <iframe></iframe>' +
            '  </div>' +
            '</div>';
        this.eleIframeContainer.setAttribute('id', 'fdrive-apps-iframe');
        this.eleIframe = this.eleIframeContainer.querySelector('iframe');
        this.eleIframeAddress = this.eleIframeContainer.querySelector('.browser-address-bar input[type=text]');
        this.eleIframeFavicon = this.eleIframeContainer.querySelector('.browser-address-bar img');
        this.eleIframeContainer.querySelector('.close-button').addEventListener('click', () => this.closeIframe());
        this.eleIframeContainer.querySelector('.open-new-tab').addEventListener('click', () => {
            let url = this.eleIframeAddress.value;
            if(!url)
                return;
            window.open(url, "_blank", "noopener,noreferrer");
        });
        // this.eleIframe.setAttribute('sandbox', 'allow-scripts');
        
        document.querySelector('.dashboard-main').appendChild(this.eleIframeContainer);
        
        this.initApps();
    }
    
    initApps(){
        for(let app of this.container.querySelectorAll('.drive-app'))
        {
            app.addEventListener('click', () => {
               this.openApp(app) 
            });
        }
    }

    hide(){
        this.closeIframe();
    }

    show(){
        if(this.initDone)
            return;
        this.initDone = true;
    }

    clear(){
        this.container.innerHTML = '';
    }
    
    async openApp(app)
    {
        for(let ele of this.container.querySelectorAll('.drive-app.selected'))
            ele.classList.remove('selected');

        app.classList.add('selected');
        
        let type = app.getAttribute('data-app-type').toLowerCase();
        
        let url = app.getAttribute('data-src');
        if(type === 'vnc'){
            const regex = /^((?:[a-fA-F0-9]{1,4}:){7}[a-fA-F0-9]{1,4}|(?:[a-fA-F0-9]{1,4}:)*:[a-fA-F0-9]{1,4}|(?:\d{1,3}\.){3}\d{1,3}|[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*)(?::(\d{1,5}))?$/;

            const match = regex.exec(url);
            const hostname = match[1];
            const port = match[2] || 5900;
            //url = `/NoVNC/vnc_lite.html?host=${encodeURIComponent(hostname)}&port=${encodeURIComponent(port)}&scale=true`;
            url = `/NoVNC/vnc_lite.html?scale=true&path=websockify/${encodeURIComponent(hostname)}/${encodeURIComponent(port)}`;
        }
        else if(type === 'external')
        {
            window.open(url, "_blank", "noopener,noreferrer");
            return;
        }
        else if(type === 'externalsame')
        {
            let a = document.createElement('a');
            a.setAttribute('href', url);
            a.setAttribute('target', 'fenrus-popup');
            a.style.display ='none';
            document.body.appendChild(a);
            a.click();
            a.remove();
            return;
        }
        else if(type === 'internal'){
            this.divLaunchingApp.querySelector('.title').textContent = 'Launching ' + app.querySelector('.name').textContent;
            this.divLaunchingApp.querySelector('img').src = app.querySelector('img').src;
            this.divLaunchingApp.style.display = 'unset';
            window.location.href = url;
            return;
        }
        this.eleIframe.src = url;
        this.eleIframeAddress.value = url;
        
        this.eleIframeFavicon.src = app.querySelector('img').src;

        this.eleIframeContainer.className = 'visible';
        document.body.classList.add('drawer-item-opened');
    }

    closeIframe(){
        this.eleIframeContainer.className = '';
        for(let ele of this.container.querySelectorAll('.email.selected'))
            ele.classList.remove('selected');
        document.body.classList.remove('drawer-item-opened');
    }
}