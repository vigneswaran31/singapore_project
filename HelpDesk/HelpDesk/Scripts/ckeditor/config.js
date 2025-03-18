/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.resize_enabled = false;
    config.extraPlugins = 'imagepaste';
    config.toolbar = 'Full';
    config.filebrowserBrowseUrl = '/scripts/ckeditor/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/Admin/Settings/BrowseImage';
    config.filebrowserUploadUrl = '/Admin/Settings/CKEditorUpload'
    //config.contentsCss = ['/content/Site/style.css', '/content/Site/common.css'];
    config.removeButtons = 'Flash';
    config.disableNativeSpellChecker = false;
    config.defaultLanguage = 'en-gb';
    config.scayt_autoStartup = true;
    config.allowedContent = true;
    config.extraAllowedContent = '*{*}';
    config.toolbar_Full =
    [{ name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'DocProps', 'Preview', 'Print', '-'/*, 'Templates'*/] },
    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
    {name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton'] },
    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
    { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
    { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote'] },
    { name: 'alignment', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },
    { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
    { name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', /* 'Iframe' */] },
    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    { name: 'colors', items: ['TextColor', 'BGColor'] }];


    config.toolbar_Basic =
    [
     { name: 'document', items: ['Source', '-', 'Preview', '-'/*, 'Templates'*/] },
     { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
     { name: 'colors', items: ['TextColor', 'BGColor'] },
     { name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', /* 'Iframe' */] },
     { name: 'alignment', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] }
    ];

};
