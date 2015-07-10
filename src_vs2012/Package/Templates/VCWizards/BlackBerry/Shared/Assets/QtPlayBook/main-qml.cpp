#include <QtGui/QApplication>
#include <QtGui/QPushButton>
#include <QtDeclarative/QtDeclarative>


#ifndef HAS_QT_QML
#error Missing Qt QML libraries. Launch NuGet Package Manager Console and type 'Install-Package codetitans-playbook-qt4-qml'.
#endif

/**
 * Application Entry Point.
 */
int main(int argc, char** argv)
{
    QCoreApplication::addLibraryPath("app/native/lib");
    QApplication app(argc, argv);
    QDeclarativeView window;

    window.setSource(QString("app/native/assets/start.qml"));
    window.show();
    return app.exec();

}
