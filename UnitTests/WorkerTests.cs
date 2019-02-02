using System;
using System.Collections.Generic;
using Engine;
using Engine.Exceptions;
using Engine.Interfaces;
using Moq;
using Xunit;

namespace UnitTests
{
    public class WorkerTests
    {
        private readonly Mock<IBairdLogger> _bairdLoggerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IImageFinder> _imageFinderMock;
        private readonly Mock<IImageMover> _imageMoverMock;
        private readonly Mock<IXmlFinder> _xmlFinderMock;
        private readonly Mock<IZipper> _zipperMock;
        
        public WorkerTests()
        {
            _bairdLoggerMock = new Mock<IBairdLogger>();
            _emailSenderMock = new Mock<IEmailSender>();
            _imageFinderMock = new Mock<IImageFinder>();
            _imageMoverMock = new Mock<IImageMover>();
            _xmlFinderMock = new Mock<IXmlFinder>();
            _zipperMock = new Mock<IZipper>();
        }

        [Fact]
        public void GivenXmlFinderCantFindTheXml_ThrowsException()
        {
            //arrange
            var exceptionToThrow = new NoXml("hello world");
            _xmlFinderMock.Setup(m => m.LocateFullXmlPath()).Throws(exceptionToThrow);
            
            //act
            var sut = Ctor();
            var exception = Assert.Throws<NoXml>(() => sut.DoWork());

            //assert
            _xmlFinderMock.Verify(m => m.LocateFullXmlPath(), Times.Once());
            
            _emailSenderMock.Verify(m => m.SendEmail(It.IsAny<string>()), Times.Never);
            _imageMoverMock.Verify(m => m.MoveImage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _imageFinderMock.Verify(m => m.LocateImageNamesFromIndexFile(It.IsAny<string>()), Times.Never);
            _zipperMock.Verify(m => m.ZipFile(It.IsAny<string>()), Times.Never);
            
            Assert.Equal(exception.Message, exceptionToThrow.Message);
        }

        [Fact]
        public void GivenXmlFinderThrowsException_ThrowsException()
        {
            //arrange
            var exceptionToThrow = new MoreThanOneXml("hello world");
            _xmlFinderMock.Setup(m => m.LocateFullXmlPath()).Throws(exceptionToThrow);
            
            //act
            var sut = Ctor();
            var exception = Assert.Throws<MoreThanOneXml>(() => sut.DoWork());

            //assert
            _xmlFinderMock.Verify(m => m.LocateFullXmlPath(), Times.Once());
            
            _emailSenderMock.Verify(m => m.SendEmail(It.IsAny<string>()), Times.Never);
            _imageMoverMock.Verify(m => m.MoveImage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _imageFinderMock.Verify(m => m.LocateImageNamesFromIndexFile(It.IsAny<string>()), Times.Never);
            _zipperMock.Verify(m => m.ZipFile(It.IsAny<string>()), Times.Never);
            
            Assert.Equal(exception.Message, exceptionToThrow.Message);
        }
        
        [Fact]
        public void GivenImageFinderThrowsException_ThrowsException()
        {
            //arrange
            var fullPathOfXml = "BobbyBairds fun house";
            var exceptionToThrow = new Exception("hello world");
            _xmlFinderMock.Setup(m => m.LocateFullXmlPath()).Returns(fullPathOfXml);
            _imageFinderMock.Setup(m => m.LocateImageNamesFromIndexFile(It.IsAny<string>())).Throws(exceptionToThrow);
            
            //act
            var sut = Ctor();
            var exception = Assert.Throws<Exception>(() => sut.DoWork());

            //assert
            _xmlFinderMock.Verify(m => m.LocateFullXmlPath(), Times.Once());
            _imageFinderMock.Verify(m => m.LocateImageNamesFromIndexFile(fullPathOfXml), Times.Once);
            
            _emailSenderMock.Verify(m => m.SendEmail(It.IsAny<string>()), Times.Never);
            _imageMoverMock.Verify(m => m.MoveImage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _zipperMock.Verify(m => m.ZipFile(It.IsAny<string>()), Times.Never);
            
            Assert.Equal(exception.Message, exceptionToThrow.Message);
        }
        
        [Fact]
        public void GivenImageFinderReturnsTwoImages_GivenImageMoverThrowsExceptionForOne_ThenBairdLoggerIsCalled()
        {
            //arrange
            var fullPathOfXml = "BobbyBairds fun house";
            var exceptionToThrow = new Exception("hello world");
            _xmlFinderMock.Setup(m => m.LocateFullXmlPath()).Returns(fullPathOfXml);
            _imageFinderMock.Setup(m => m.LocateImageNamesFromIndexFile(It.IsAny<string>())).Returns(new List<string>(){"image0", "image1"});
            _imageMoverMock.SetupSequence(m => m.MoveImage(It.IsAny<string>(), It.IsAny<string>())).Pass()
                .Throws(exceptionToThrow);
            
            //act
            var sut = Ctor();
            sut.DoWork();

            //assert
            _xmlFinderMock.Verify(m => m.LocateFullXmlPath(), Times.Once());
            _imageFinderMock.Verify(m => m.LocateImageNamesFromIndexFile(fullPathOfXml), Times.Once);
            _imageMoverMock.Verify(m => m.MoveImage(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _bairdLoggerMock.Verify(m => m.LogException(exceptionToThrow), Times.Once);
            
            _emailSenderMock.Verify(m => m.SendEmail(It.IsAny<string>()), Times.Once);
            _zipperMock.Verify(m => m.ZipFile(It.IsAny<string>()), Times.Once);
        }
        
        

        private Worker Ctor()
        {
            return new Worker(_bairdLoggerMock.Object, _emailSenderMock.Object, _imageFinderMock.Object, _imageMoverMock.Object, _xmlFinderMock.Object, _zipperMock.Object);
        }
    }
}