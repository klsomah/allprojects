var fs = require('fs');
buildJSFilePath="build.js";
appSCSSFilePath="app/scss/app.scss";
appConfigureJSFilePath="app.configure.js";
gulpDeployJSFilePath="gulp-deploy.js";
leftNavbarDemoFilePath="LeftNavbarDemo";
topNavbarDemoFilePath="TopNavbarDemo";
deployFilePath="deploy";
appFilePath="";
demoFolderPath="";

topLayout='layout-top-navigation';
leftLayout='layout-side-navigation';
layout=topLayout;
const execSync = require('child_process').execSync;


build();
function build(){
    initialize();
    console.log("automate build starts");
    changeOutputDirectoryName();
    changeTheme();
    runGulpCommands();
    runBuildCommand();
    renameDeployFolderName();
    // moveFoldersAndFiles();
    // cleanUp();
    console.log("automate build completed!");
}

function initialize(){
    if(layout==leftLayout){
        demoFolderPath=leftNavbarDemoFilePath;
        appFilePath=leftNavbarDemoFilePath+"/app";
    }        
    if(layout==topLayout){
        demoFolderPath=topNavbarDemoFilePath;
        appFilePath=topNavbarDemoFilePath+"/app";
    }
}

function readFile(filePath){
    var data=fs.readFileSync(filePath).toString();
    return data;
}

function replaceTextInFile(filePath,searchText,replaceText){
    var data=fs.readFileSync(filePath).toString();;
    data=data.replace(searchText,replaceText);
    fs.writeFileSync(filePath,data);
}

//In build.js -> change outputDirectoryName to deploy
function changeOutputDirectoryName(){
    replaceTextInFile(buildJSFilePath,"var outputDirectoryName=constants.out;","var outputDirectoryName=constants.deploy;");
}

//In app.scss -> Comment '@import 'layout-top-navigation';' and uncomment '@import 'layout-side-navigation';'.										
function changeLayoutInAppSCSS(){
    var data=readFile(appSCSSFilePath);
    if(layout==leftLayout && data.indexOf("//@import 'layout-top-navigation';")==-1){
        replaceTextInFile(appSCSSFilePath,"@import 'layout-top-navigation';","//@import 'layout-top-navigation';");
        replaceTextInFile(appSCSSFilePath,"//@import 'layout-side-navigation';","@import 'layout-side-navigation';");
        return;
    }   
    if(layout==topLayout && data.indexOf("//@import 'layout-side-navigation';")==-1){
        replaceTextInFile(appSCSSFilePath,"@import 'layout-side-navigation';","//@import 'layout-side-navigation';");
        replaceTextInFile(appSCSSFilePath,"//@import 'layout-top-navigation';","@import 'layout-top-navigation';");
        return;
    }
}

//In app.config.js -> 'defaultLayout' should be 'layout'	
function changeLayoutInAppConfigureJS(){
    if(layout==topLayout)
        replaceTextInFile(appConfigureJSFilePath,"var defaultLayout = layout;","var defaultLayout = layoutTopNavigation;");
    
    if(layout==leftLayout)
        replaceTextInFile(appConfigureJSFilePath,"var defaultLayout = layoutTopNavigation;","var defaultLayout = layout;");

}	

//In gulp-deploy.js -> Remove rgBasePath+'rg-layout-top-navigation.js' and keep rgBasePath+'rg-layout.js'	
function changeLayoutInGulpDeployJS(){
    if(layout==topLayout)   
        replaceTextInFile(gulpDeployJSFilePath,"rgBasePath+'rg-layout.js'","rgBasePath+'rg-layout-top-navigation.js'");

    if(layout==leftLayout)
    replaceTextInFile(gulpDeployJSFilePath,"rgBasePath+'rg-layout-top-navigation.js'","rgBasePath+'rg-layout.js'");
}

function changeTheme(){
    changeLayoutInAppSCSS();
    changeLayoutInAppConfigureJS();
    changeLayoutInGulpDeployJS();
}

//Run 'gulp scss'										
//Run 'gulp allCSSTasks'										
//Run 'gulp combineJS'										
function runGulpCommands(){
    console.log("Running gulp scss...")
    execSync('gulp scss');
    console.log("Running gulp allCSSTasks...")
    execSync('gulp allCSSTasksForLTR');
    console.log("Running gulp combineJS...")
    execSync('gulp combineJS');    
}

//Run 'node build'
function runBuildCommand(){
    console.log("Running node build...")
    execSync('node build');
}

//Rename 'deploy' folder name by 'LeftNavbarDemo'
function renameDeployFolderName(){
    if(layout==leftLayout)
        fs.renameSync(deployFilePath,leftNavbarDemoFilePath);
    if(layout==topLayout)
        fs.renameSync(deployFilePath,topNavbarDemoFilePath);
}

//In newly created LeftNavbarDemo folder, Move all folders from 'app' to root folder
//Move 'webfonts' folder from vender to root folder
function moveFoldersAndFiles(){
    fs.rename(appFilePath, demoFolderPath, function (err) {
        if (err) throw err
        console.log('Successfully renamed - AKA moved!')
      })
}

//Delete app folder
//Remove gulp file from 'LeftNavbarDemo'
function cleanUp(){

}