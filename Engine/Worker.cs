using System;
using Engine.Interfaces;

namespace Engine
{
    public class Worker
    {
        private IBairdLogger _bairdLogger;
        private IEmailSender _emailSender;
        private IImageFinder _imageFinder;
        private IImageMover _imageMover;
        private IXmlFinder _xmlFinder;
        private IZipper _zipper;

        public Worker()
        {
                
        }
        
        public Worker(IBairdLogger bairdLogger, IEmailSender emailSender, IImageFinder imageFinder, IImageMover imageMover, IXmlFinder xmlFinder, IZipper zipper)
        {
            _bairdLogger = bairdLogger;
            _emailSender = emailSender;
            _imageFinder = imageFinder;
            _imageMover = imageMover;
            _xmlFinder = xmlFinder;
            _zipper = zipper;
        }
        
        public void DoWork()
        {
            var fullPathOfXml = _xmlFinder.LocateFullXmlPath();

            var listOfImageNames = _imageFinder.LocateImageNamesFromIndexFile(fullPathOfXml);

            foreach (var image in listOfImageNames)
            {
                try
                {
                    _imageMover.MoveImage(image, "to/happy/land");
                }
                catch (Exception exception)
                {
                    _bairdLogger.LogException(exception);
                }
            }

            var zipFilePath = _zipper.ZipFile("something");
            
            _emailSender.SendEmail(zipFilePath);
        }
    }
}