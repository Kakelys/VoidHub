import { Component, ElementRef, Renderer2, ViewChild} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  @ViewChild('html') htmlEl:ElementRef;

  constructor(private trans: TranslateService, private render: Renderer2) {
    this.langInit();
    this.themeInit();
  }

  langInit() {
    this.trans.setDefaultLang('en');
    let savedLocale = localStorage.getItem('locale');
    if(savedLocale)
      this.trans.use(savedLocale);
    else
      localStorage.setItem('locale', 'en');
  }

  changeLanguage(code: string) {
    this.trans.use(code);
    localStorage.setItem('locale', code);
  }

  themeInit() {
    const storedTheme = localStorage.getItem('theme');
    let currentTheme = document.getElementById("html").getAttribute('data-theme')

    if(!storedTheme || currentTheme == storedTheme)
      return;

    this.changeTheme(storedTheme);
  }

  changeTheme(name: string){
    this.render.setAttribute(document.getElementById("html"), 'data-theme', name);
    localStorage.setItem('theme', name);
  }
}
