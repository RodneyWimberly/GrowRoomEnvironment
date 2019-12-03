// =============================
// Email: info@wimberlytech.com
// www.wimberlytech.com/templates
// =============================

import { AppPage } from './app.po';
import { browser, logging } from 'protractor';

describe('workspace-project App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: GrowRoomEnvironment', () => {
    page.navigateTo();
    expect(page.getAppTitle()).toEqual('GrowRoomEnvironment');
  });
});
